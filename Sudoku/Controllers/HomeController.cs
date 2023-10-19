using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            if (Request.Cookies["token"] == null)
                return RedirectToAction("Login", "Users");

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