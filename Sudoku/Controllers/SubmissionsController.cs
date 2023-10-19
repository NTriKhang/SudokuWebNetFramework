using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sudoku.DAL;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.Controllers
{
    public class SubmissionsController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        private SudokuService sudokuService = new SudokuService();

        // GET: Submissions
        public ActionResult Index(string chapterId)
        {
            var userId = userService.GetUserId(Request);
            var submissions = db.Submissions.Where(x => x.ChapterId == chapterId && x.UserId == userId).OrderByDescending(x => x.SubmitDate).Include(s => s.Chapter);
            return View(submissions.ToList());
        }

        // GET: Submissions/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Submission submission = await db.Submissions.Where(x => x.ID == id)
                                                        .Include(x => x.Chapter)
                                                        .FirstOrDefaultAsync();
            
            submission.Chapter.TypeNavigation = await db.ChapterTypes.Where(x => x.Id == submission.Chapter.Type_id).FirstOrDefaultAsync();
            string path = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Submission", submission.Submit_path_file);
            string sudokuString = sudokuService.ReadSudokuStringFromFile(path);
            if (submission == null)
            {
                return HttpNotFound();
            }
            SubmissionDetailDto submissionDetail = new SubmissionDetailDto()
            {
                ChapterName = submission.Chapter.Name,
                CompletedPercent = submission.CompletedPercent,
                State = submission.State,
                NameType = submission.Chapter.TypeNavigation.Name,
                SubmitDate = submission.SubmitDate,
                SudokuString = sudokuString
            };
            return View(submissionDetail);
        }

        [HttpGet]
        public async Task<ActionResult> UserSubmiss(int page = 1)
        {
            if (Request.Cookies["token"] == null)
            {
                return HttpNotFound();
            }
            var userId = userService.GetUserId(Request);

            int skipPage = 7 * (page - 1);
            int submisPerPage = 7 * page;

            var submisSuccess = await db.Submissions.Where(x => x.UserId == userId)
                                                    .OrderByDescending(x => x.SubmitDate)
                                                    .Skip(skipPage)
                                                    .Take(submisPerPage)
                                                    .Include(x => x.Chapter)
                                                    .ToListAsync();

            return View(submisSuccess);
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
