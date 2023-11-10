using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Sudoku.DAL;
using Sudoku.Dtos.Incoming;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.Controllers
{
    public class Room
    {
        public Room()
        {
            UsersId = new List<string>();
        }
        public string roomOwnId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Point { set; get; }
        public string Level { set; get; }
        public string Riddle { set; get; }
        public bool BeginState { set; get; } // true if it have started
        public int PointReward { set; get; }
        public List<string> UsersId { set; get; }
    }
    public class RoomDto
    {
        public string roomId { set; get; }
        public string roomOwnId { set; get; }
        public string userId { set; get; }
        public string isJoin { set; get; } = "false";

    }
    public class SudokuSection
    {
        public string roomId { set; get; }
        public string riddle { set; get; }
        public int rows { set; get; }
    }
    //
    public class ContestsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        private RoomService roomService = new RoomService();
        private SudokuService sudokuService = new SudokuService();
        private SimulatedAnnealing simulated = new SimulatedAnnealing();
        private static List<Room> rooms { set; get; } = new List<Room>();

        // GET: Contests
        public ActionResult Index()
        {
            if (Request.Cookies["token"] == null)
                return RedirectToAction("Login", "users");

            //var userId = userService.GetUserId(Request);

            //var userCurrentRoom = rooms.Where(x => x.UsersId.Contains(userId)).FirstOrDefault();
            //if (userCurrentRoom != null)
            //{
            //    Session["UserCurrentRoom"] = userCurrentRoom.Id;
            //}
            if (Session["UserCurrentRoom"] != null)
            {
                var isRoomExist = rooms.Any(x => x.Id == Session["UserCurrentRoom"].ToString());
                if (isRoomExist == false)
                    Session["UserCurrentRoom"] = null;
            }
            List<ContestDto> contestDtos = roomService.GetContests(rooms);
            return View(contestDtos);
        }
        [HttpGet]
        public ActionResult Room(string roomId)
        {
            var room = rooms.Where(x => x.Id == roomId).SingleOrDefault();
            if (room == null)
            {
                List<ContestDto> contestDtos = roomService.GetContests(rooms);
                ViewBag.Conflict = "This room have dispersed";
                return View("Index", contestDtos);
            }
            var userId = userService.GetUserId(Request);
            if (!room.UsersId.Contains(userId) && room.BeginState)
            {
                ViewBag.Conflict = "This room has begun";
                var contestDtos = roomService.GetContests(rooms);
                return View("Index", contestDtos);
            }

            var roomDto = new RoomDto();
            roomDto.roomId = roomId;
            roomDto.roomOwnId = room.roomOwnId;
            roomDto.userId = userId;
            if (Session["UserCurrentRoom"] != null && Session["UserCurrentRoom"].ToString() == roomId)
            {
                roomDto.isJoin = "true";
            }
            return View(roomDto);
        }
        [HttpPost]
        public async Task<ActionResult> JoinRoom(string roomId)
        {
            var userId = userService.GetUserId(Request);

            // if user in is any room, force user to quit first
            var userRoom = rooms.Where(x => x.UsersId.Contains(userId)).SingleOrDefault();

            if (userRoom != null)
            {
                if (userRoom.Id == roomId)
                {
                    //if user have already joined this room so just return
                    return Json(new { code = 200 });
                }
                return Json(new { code = 409, msg = "You need to quit your room first" });
            }
            //consider if your point is enough or not
            var point = await db.Users.Where(x => x.Id == userId).Select(x => x.Point).FirstOrDefaultAsync();
            var currentRoom = rooms.Where(x => x.Id == roomId).SingleOrDefault();

            if (currentRoom == null)
                return Json(new { code = 409, msg = "Room does not exits anymore" });

            if (currentRoom.BeginState)
                return Json(new { code = 400, msg = "Room has begun" });
            else if (point < currentRoom.Point)
                return Json(new { code = 409, msg = "Your point is not enough" });
            else
            {
                currentRoom.UsersId.Add(userId);
                Session["UserCurrentRoom"] = roomId;
                return Json(new { code = 200 });
            }
        }
        [HttpGet]
        public async Task<ActionResult> Start()
        {
            var userId = userService.GetUserId(Request);
            if (!roomService.IsOwnRoom(rooms, userId))
            {
                return Json(new { code = 400 }, JsonRequestBehavior.AllowGet);
            }
            var room = rooms.FirstOrDefault(x => x.roomOwnId == userId);
            if (room.BeginState)
            {
                room.UsersId.Remove(userId);
                return Json(new {code = 400, msg="Room has begun"}, JsonRequestBehavior.AllowGet);
            }
            room.BeginState = true;
            room.PointReward = room.UsersId.Count * room.Point;

            await MinusUserPoint(room.UsersId, room.Point);

            return Json(new { code = 200 }, JsonRequestBehavior.AllowGet);
        }
        [NonAction] 
        private async Task MinusUserPoint(List<string> usersId, int roomPoint)
        {
            foreach (string playerId in usersId)
            {
                var player = await db.Users.FindAsync(playerId);
                player.Point -= roomPoint;
            }
            await db.SaveChangesAsync();
        }
        [HttpPost]
        public ActionResult QuitRoom(string roomId)
        {
            var userId = userService.GetUserId(Request);
            var roomOwnId = roomService.GetRoomOwnId(rooms, roomId);
            var room = rooms.SingleOrDefault(x => x.Id == roomId);
            Session["UserCurrentRoom"] = null;
            if (userId == roomOwnId && rooms.Where(x => x.Id == roomId).Single().BeginState == false)
            {
                rooms.Remove(room);
                return Json(new { code = 200 });
            }
            else
            {
                room.UsersId.Remove(userId);
                if (room.UsersId.Count == 0)
                {
                    rooms.Remove(room);
                    return Json(new { code = 200 });
                }
                return Json(new { code = 409 });
            }
        }
        [HttpGet]
        public async Task<ActionResult> UsersSide(string roomId)
        {
            List<UserDto> users = new List<UserDto>();
            var room = rooms.Find(x => x.Id == roomId);
            foreach (var userId in room.UsersId)
            {
                var user = await db.Users.FindAsync(userId);
                users.Add(new UserDto()
                {
                    Id = user.Id,
                    First_name = user.First_name,
                    Last_name = user.Last_name,
                    Profile_Image = "\\" +  Path.Combine("wwwroot", "ProfileImage", user.Profile_Image),
                    Email = user.Email,
                    Point = user.Point,
                });
            }
            ViewBag.UserId = userService.GetUserId(Request);
            ViewBag.RoomOwnId = roomService.GetRoomOwnId(rooms, roomId);
            return View(users);
        }
        [HttpPost]
        public ActionResult SudokuSide(SudokuSection sudoku)
        {
            var point = Session["Point"] as int?;
            point -= rooms.Where(x => x.Id == sudoku.roomId).Select(x => x.Point).SingleOrDefault();
            Session["Point"] = point;
            return View(new SudokuSection { riddle = sudoku.riddle, rows = sudoku.rows });
        }
        [HttpGet]
        public async Task<ActionResult> GenerateRiddle(string roomId)
        {
            string level = rooms.Where(x => x.Id == roomId).Select(x => x.Level).SingleOrDefault();
            int rows = (level == utility.LevelHard) ? 9 : (level == utility.LevelMedium) ? 7 : 5;

            var annealing = new SimulatedAnnealing();
            var riddle = await annealing.getRandomRiddle(rows);
            var sudokuStr = sudokuService.FormatArrayToString(riddle);
            return Json(new { code = 200, riddle = sudokuStr, rows = rows }, JsonRequestBehavior.AllowGet);
        }
        // GET: Contests/Create
        public async Task<ActionResult> Create()
        {
            if (Request.Cookies["token"] == null)
                return RedirectToAction("Login", "Users");

            var userId = userService.GetUserId(Request);

            if (roomService.IsOwnRoom(rooms, userId))
            {
                ViewBag.Conflict = "Need to disperse your room first";
                var contestDtos = roomService.GetContests(rooms);
                return View("Index", contestDtos);
            }

            var items = await db.ChapterTypes.OrderByDescending(x => x.Point).Select(selector => new SelectListItem
            {
                Text = selector.Name,
                Value = selector.Name,

            }).ToListAsync();
            ViewBag.Level = items;

            return View();
        }

        // POST: Contests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContestCreateDto contest)
        {
            var userId = userService.GetUserId(Request);
            var user = await db.Users.FindAsync(userId);
            List<SelectListItem> items;

            if (ModelState.IsValid)
            {
                if (contest.Point > user.Point)
                {
                    items =  await db.ChapterTypes.OrderByDescending(x => x.Point).Select(selector => new SelectListItem
                    {
                        Text = selector.Name,
                        Value = selector.Name,

                    }).ToListAsync();
                    ViewBag.Level = items;
                    ViewBag.Error = "Point room have to less than your individual point";
                    return View();
                }

                var room = new Room();
                room.UsersId.Add(userId);
                room.roomOwnId = userId;
                room.Name = contest.Name;
                room.Level = contest.Level;
                room.Point = contest.Point;
                room.Id = Guid.NewGuid().ToString();
                room.Riddle = "";
                room.BeginState = false;
              
                rooms.Add(room);
                Session["UserCurrentRoom"] = room.Id;

                return RedirectToAction("Room", new { roomId = room.Id });
            }

            items = await db.ChapterTypes.OrderByDescending(x => x.Point).Select(selector => new SelectListItem
            {
                Text = selector.Name,
                Value = selector.Name,

            }).ToListAsync();
            ViewBag.Level = items;
            return View(contest);
        }
        [HttpPost]
        public async Task<ActionResult> Submit(string sudokuString, int rows, string roomId)
        {
            char[,] sudokuArr = sudokuService.FormatStringToArr(sudokuString, rows, rows);
            double percent = sudokuService.CaculateCompletePercent(sudokuArr);
            if (percent == 100)
            {
                var userId = userService.GetUserId(Request);
                var user = await db.Users.FindAsync(userId);

                var responseData = new
                {
                    userName = user.First_name + ' ' + user.Last_name,
                    imagePath = "\\" + Path.Combine("wwwroot", "ProfileImage", user.Profile_Image)
                };

                var room = rooms.SingleOrDefault(x => x.Id == roomId);

                user.Point += room.PointReward;
                Session["Point"] = user.Point;
                Contest contest = new Contest()
                {
                    Name = room.Name,
                    DateTime = DateTime.Now,
                    Level = room.Level,
                    Point = room.Point
                };
                List<JoinContest> joinContestList = new List<JoinContest>();
                foreach(var playerId in room.UsersId)
                {
                    var joinContest = new JoinContest()
                    {
                        ContestId = contest.Id,
                        UserId = playerId,
                        State = (playerId == userId) ? true : false,
                    };
                    joinContestList.Add(joinContest);
                }
                db.Contests.Add(contest);
                db.JoinContests.AddRange(joinContestList);

                await db.SaveChangesAsync();

                rooms.Remove(room);
                return Json(new { code = 200 , userName = responseData.userName, imagePath = responseData.imagePath});
            }
            return Json(new { code = 400 });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
