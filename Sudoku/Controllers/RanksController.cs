using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Sudoku.DAL;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.Controllers
{
    public class RanksController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        // GET: Ranks
        public ActionResult Index()
        {
            var date = DateTime.Now.AddMinutes(-utility.RankTime);
            var ranks = db.Ranks.Where(x => x.DateRank > date)
                                .OrderBy(x => x.Value)
                                .Take(4)
                                .Include(r => r.User);
            ranks.ForEach(x =>
            {
                x.User.Profile_Image = "\\" + Path.Combine("wwwroot", "ProfileImage", x.User.Profile_Image);
            });
            return View(ranks.ToList());
        }
        [HttpGet]
        public async Task<ActionResult> UserRank()
        {
            var userId = userService.GetUserId(Request);
            var date = DateTime.Now.AddMinutes(-utility.RankTime);
            var rank = await db.Ranks.Where(x => x.UserId == userId && x.DateRank > date).Include(x => x.User).SingleOrDefaultAsync();

            if(rank == null)
            {
                rank = new Rank();
                rank.SubmitCount = 0;
                rank.Value = 0;
                rank.User = await db.Users.FindAsync(userId);
            }
            rank.User.Profile_Image = "\\" + Path.Combine("wwwroot", "ProfileImage", rank.User.Profile_Image);

            return View(rank);
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
