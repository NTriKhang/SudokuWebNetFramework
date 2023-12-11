using Sudoku.DAL;
using Sudoku.Dtos.Incoming;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    public class DiscussionsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        // GET: Discussions
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult UserDiscussion(int page = 1)
        {
            if (Request.Cookies["token"] == null)
            {
                return Json(null);
            }

            int skipPage = 7 * (page - 1);  
            int discussPerPage = 7 * page;

            var userId = userService.GetUserId(Request);
            var discussions = db.Discussions.Where(x => x.UserId == userId)
                                            .OrderByDescending(x => x.Date)
                                            .Skip(skipPage)
                                            .Take(discussPerPage)
                                            .Include(x => x.Chapter)
                                            .ToList();
            return View(discussions);
        }
        [HttpGet]
        public ActionResult ChapterDiscussion(string chapterId)
        {
            ViewBag.ChapterId = chapterId;
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> ChapterDiscussion(DiscussSubmitDto discussSubmitDto)
        {
            if(ModelState.IsValid)
            {
                string userId = userService.GetUserId(Request);
                var discussion = new Discussion()
                {
                    Content = discussSubmitDto.TextContent,
                    Date = DateTime.Now,
                    ChapterId = discussSubmitDto.ChapterId,
                    UserId = userId
                };

                db.Discussions.Add(discussion);
                await db.SaveChangesAsync();
                return RedirectToAction("SubChapterDiscussion", new {chapterId = discussion.ChapterId});
            }
            else
            {
                var query = db.Discussions.Where(x => x.ChapterId == discussSubmitDto.ChapterId);
                if (query.Any())
                {
                    var res = await query.OrderByDescending(x => x.Date).Include(x => x.User).ToListAsync();
                    return View(res);
                }
                return View(query.ToList());
            }

        }
        [HttpGet]
        public async Task<ActionResult> SubChapterDiscussion(string chapterId)
        {
            var query = db.Discussions.Where(x => x.ChapterId == chapterId);
            var response = new DiscussionDto()
            {
                ChapterId = chapterId,
                CurrentUserId = userService.GetUserId(Request)
            };

            if (query.Any())
            {
                response.Discuss = await query.OrderByDescending(x => x.Date).Include(x => x.User)
                                     .Select(x => new CDiscuss
                                     {
                                         TextContent = x.Content,
                                         Date = x.Date,
                                         UserName = x.User.First_name + " " + x.User.Last_name,
                                         UserId = x.User.Id,
                                         DiscussId = x.Id,
                                         UserImage_Path = "\\wwwroot\\ProfileImage\\" + x.User.Profile_Image
            }).ToListAsync();
                return View(response);
            }
            return View(response);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(DiscussDeleteDto deleteDto)
        {
            if(deleteDto.discussId != null)
            {
                var discuss = await db.Discussions.FindAsync(deleteDto.discussId);
                if(discuss != null)
                {
                    db.Discussions.Remove(discuss);
                    await db.SaveChangesAsync();
                    return RedirectToAction("SubChapterDiscussion", new { chapterId= deleteDto.chapterId });
                }
                return Json(new { code = 404, msg = "Discuss not found" });

            }
            return Json(new { code=400, msg="discussId might not null" });
        }
    }
}
