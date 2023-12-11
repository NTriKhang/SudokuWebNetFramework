using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Antlr.Runtime.Misc;
using Newtonsoft.Json.Linq;
using Sudoku.DAL;
using Sudoku.Dtos.Incoming;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.Controllers
{
    public class ChaptersController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private SudokuService sudokuService = new SudokuService();
        private UserService userService = new UserService();
        private int ChapterQuantity {  get; set; }
        public ChaptersController()
        {
        }

        // GET: Chapters
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> Chapters(int page, string chapterName = null)
        {

            var chapterPerPage = 5;
            int skipChapter = (page - 1) * chapterPerPage;
            
            List<ChapterDto> chapters;

            var userId = userService.GetUserId(Request);

            if (chapterName == "")
            {
                ChapterQuantity = await db.Chapters.CountAsync();

                chapters = await db.Chapters.OrderBy(x => x.CreateDate)
                                      .Skip(skipChapter).Take(chapterPerPage)
                                      .Include(x => x.TypeNavigation)
                                      .Select(x => new ChapterDto
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          Chap_file_path = x.Chap_file_path,
                                          CreateDate = x.CreateDate,
                                          Type_id = x.Type_id,
                                          TypeNavigation = x.TypeNavigation,
                                          IsCompleted = db.Submissions.Any(s => s.ChapterId == x.Id && s.UserId == userId && s.State == utility.StateCompleted)
                                      })
                                      .ToListAsync();
            }
            else
            {
                ChapterQuantity = await db.Chapters.Where(x => x.Name.Contains(chapterName)).CountAsync();

                chapters = await db.Chapters.Where(x => x.Name.Contains(chapterName))
                                      .OrderBy(x => x.CreateDate)
                                      .Skip(skipChapter).Take(chapterPerPage)
                                      .Include(x => x.TypeNavigation)
                                      .Select(x => new ChapterDto
                                      {
                                          Id = x.Id,
                                          Name = x.Name,
                                          Chap_file_path = x.Chap_file_path,
                                          CreateDate = x.CreateDate,
                                          Type_id = x.Type_id,
                                          TypeNavigation = x.TypeNavigation,
                                          IsCompleted = db.Submissions.Any(s => s.ChapterId == x.Id && s.UserId == userId && s.State == utility.StateCompleted)
                                      })
                                      .ToListAsync();
            }

            if (ChapterQuantity == 0)
                ViewBag.PageQuantity = 0;
            else if (ChapterQuantity < chapterPerPage)
                ViewBag.PageQuantity = 1;
            else
                ViewBag.PageQuantity = Math.Ceiling((double)ChapterQuantity / chapterPerPage);

            return View(chapters);
        }
        // GET: Chapters/Create
        public async Task<ActionResult> Upsert(string id)
        {
            var chapterCreateDto = new ChapterUpsertDto();
            var items = await db.ChapterTypes.OrderByDescending(x => x.Point).Select(selector => new SelectListItem
            {
                Text = selector.Name,
                Value = selector.Id,

            }).ToListAsync();


            chapterCreateDto.ChapterType = items;
            if(id != null)
            {
                var chapter = await db.Chapters.SingleOrDefaultAsync(x => x.Id == id);
                ViewBag.SudokuArr = sudokuService.ReadSudokuStringFromFile(chapter.Chap_file_path);
                chapterCreateDto.ChapterId = chapter.Id;
                chapterCreateDto.Name = chapter.Name;
                chapterCreateDto.Type_id = chapter.Type_id;
                chapterCreateDto.Chap_file_name = chapter.Chap_file_path;
            }

            else
            {
                char[,] sudokuArr = new char[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        sudokuArr[i, j] = '0';
                    }
                }
                chapterCreateDto.ChapterId = "";
                ViewBag.SudokuArr = sudokuService.FormatArrayToString(sudokuArr);
            }
            string userId = userService.GetUserId(Request);
            chapterCreateDto.RemainPost = await db.AuthorService.Where(x => x.UserId == userId).Select(x => x.RemainPostNumber).SingleOrDefaultAsync(); 

            ViewBag.DefaultSudoku = sudokuService.ReadSudokuStringFromFile("default-chapter.txt");
            return View(chapterCreateDto);
        }
        [HttpPost]
        public async Task<ActionResult> Upsert(ChapterUpsertDto chapterDto)
        {
            if (ModelState.IsValid)
            {

                var userId = userService.GetUserId(Request);
                var authorService = await db.AuthorService.Where(x => x.UserId == userId).SingleOrDefaultAsync();
                if(authorService.RemainPostNumber == 0)
                {
                    return Json(new { code = 409, msg = "You have run out of posts for today" });
                }
                authorService.RemainPostNumber--;

                if(chapterDto.ChapterId == null)
                {
                    var fileName = Guid.NewGuid().ToString() + ".txt";
                    await sudokuService.WriteSudokuStringToFile(fileName, chapterDto.SudokuString);

                    var chapter = new Chapter()
                    {
                        Name = chapterDto.Name,
                        Chap_file_path = fileName,
                        Type_id = chapterDto.Type_id,
                        CreateDate = DateTime.Now,
                        AuthorId = userId
                    };
                    db.Chapters.Add(chapter);
                }
                else
                {
                    await sudokuService.WriteSudokuStringToFile(chapterDto.Chap_file_name, chapterDto.SudokuString);
                    var chapter = await db.Chapters.FindAsync(chapterDto.ChapterId);
                    chapter.Type_id = chapterDto.Type_id;
                    chapter.Name = chapterDto.Name;
                }
                await db.SaveChangesAsync();
                return Json(new { code = 200 });
            }

            chapterDto.ChapterType = await db.ChapterTypes.OrderByDescending(x => x.Point).Select(selector => new SelectListItem
            {
                Text = selector.Name,
                Value = selector.Id,
            }).ToListAsync();
            return Json(new {code = 409, msg="Lack of information"});
        }

        [HttpPost]
        public ActionResult TestConflict(string sudokuString, int rows, int columns)
        {

            var sudokuArr = sudokuService.FormatStringToArr(sudokuString, rows, columns);
            return Json(sudokuService.TestSudoku(sudokuArr)); // return code 200 if success else return box that cause conflict
        }
        [HttpGet]
        public async Task<ActionResult> GenerateRiddle(int rows)
        {
            var annealing = new SimulatedAnnealing();
            var riddle = await annealing.getRandomRiddle(rows);
            var sudokuStr = sudokuService.FormatArrayToString(riddle);
            return Json(new { code = 200, sudokuString = sudokuStr }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Play(string id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> GetChapter(string chapterId)
        {
            Chapter chapter = await db.Chapters.Where(x => x.Id == chapterId).Include(nameof(Chapter.TypeNavigation)).FirstOrDefaultAsync();

            ChapterPlayDto chapterPlayDto = new ChapterPlayDto()
            {
                chapter = chapter,
                sudokuArr = sudokuService.ReadSudokuStringFromFile(chapter.Chap_file_path)
            };
            return View(chapterPlayDto);
        }
        [HttpPost]
        public async Task<ActionResult> Play(SubmitDto submitDto)
        {
            var type = submitDto.Type_Name;
            char[,] sudokuArr;

            var userId = userService.GetUserId(Request);

            if (type == "Easy")
                sudokuArr = sudokuService.FormatStringToArr(submitDto.SudokuString, 5, 5);
            else if (type == "Medium")
                sudokuArr = sudokuService.FormatStringToArr(submitDto.SudokuString, 7, 7);
            else
                sudokuArr = sudokuService.FormatStringToArr(submitDto.SudokuString, 9, 9);

            string submitFileName = Guid.NewGuid().ToString().Substring(0, 6);
            string submitPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Submission", submitFileName);
            await sudokuService.WriteSudokuStringToFile(submitPath, submitDto.SudokuString);

            bool isCompleted = await db.Submissions.AnyAsync(x => x.ChapterId == submitDto.ChapterId && x.UserId == userId && x.State == utility.StateCompleted);
            double percent = sudokuService.CaculateCompletePercent(sudokuArr);

            if (!isCompleted && percent == 100)
            {
                var user = await db.Users.FindAsync(userId);

                int chapterPoint = await db.Chapters.Where(x => x.Id == submitDto.ChapterId)
                                                .Include(x => x.TypeNavigation)
                                                .Select(x => x.TypeNavigation.Point)
                                                .SingleOrDefaultAsync();

                user.Point += chapterPoint;
                Session["Point"] = user.Point;
            }

            Submission submission = new Submission()
            {
                UserId = userId,
                ChapterId = submitDto.ChapterId,
                CompletedPercent = Convert.ToDouble(String.Format("{0:0.00}", percent)),
                State = (percent == 100) ? utility.StateCompleted : utility.StateInComplete,
                SubmitDate = DateTime.Now,
                Submit_path_file = submitFileName
            };
            db.Submissions.Add(submission);
            await db.SaveChangesAsync();
            return RedirectToAction("Index","Submissions", new {chapterId = submission.ChapterId});
        }
        [HttpGet]
        public async Task<ActionResult> AuthorFeatures(int page = 1)
        {
            var userRoleId = userService.GetUserRole(Request);
            if (userRoleId != await db.Roles.Where(x => x.Name == utility.Author).Select(x => x.Id).FirstOrDefaultAsync())
            {
                return RedirectToAction("Index", "RoleManage");
            }

            var userId = userService.GetUserId(Request);

            ChapterQuantity = await db.Chapters.Where(x => x.AuthorId == userId).CountAsync();
            var chaptersPerPage = 5;
            int skipChapter = (page - 1) * chaptersPerPage;
            var authorChapter = await db.Chapters.Where(x => x.AuthorId == userId)
                                                 .OrderBy(x => x.CreateDate)
                                                 .Skip(skipChapter)
                                                 .Take(chaptersPerPage)
                                                 .Include(x => x.TypeNavigation).ToListAsync();
            if (ChapterQuantity < chaptersPerPage)
            {
                ViewBag.PageQuantity = 1;
            }
            else
            {
                ViewBag.PageQuantity = Math.Ceiling((double)ChapterQuantity / chaptersPerPage);
            }
            return View("AuthorFeatures", authorChapter);
        }
  
        // GET: Chapters/Delete/5
        public ActionResult Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Chapter chapter = db.Chapters.Find(id);
                if (chapter == null)
                {
                    return HttpNotFound();
                }
                return View(chapter);
            }
            catch (Exception)
            {
                ViewBag["Conflict"] = "Conflict foreign key";
                throw;
            }
        }

        // POST: Chapters/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Chapter chapter = await db.Chapters.FindAsync(id);

                db.Chapters.Remove(chapter);
                string fileDeletePath = chapter.Chap_file_path;
                await db.SaveChangesAsync();

                sudokuService.DeletedSudokuFile(fileDeletePath);


                return Json(new { code = 200 });
            }
            catch (Exception)
            {
                return Json(new { code = 409 });
            }
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
