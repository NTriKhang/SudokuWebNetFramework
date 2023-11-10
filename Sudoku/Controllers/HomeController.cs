using Sudoku.DAL;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _db = new AppDbContext();
        private UserService _userService = new UserService();
        public async Task<ActionResult> Index()
        {
            if (Request.Cookies["token"] == null)
                return RedirectToAction("Login", "Users");

            var userId = _userService.GetUserId(Request);
            var point = await _db.Users.Where(x => x.Id == userId).Select(x => new
            {
                point = x.Point,
                userImage = x.Profile_Image
            }).SingleOrDefaultAsync();
            Session["Point"] = point.point;
            Session["UserImage"] = "\\" + Path.Combine("wwwroot", "ProfileImage", point.userImage);
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            var login = User.Identity.IsAuthenticated;

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}