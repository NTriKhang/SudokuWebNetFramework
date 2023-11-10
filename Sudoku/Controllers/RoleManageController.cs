using Stripe;
using Stripe.Checkout;
using Sudoku.DAL;
using Sudoku.Models;
using Sudoku.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Sudoku.Controllers
{
    public class RoleManageController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        // GET: RoleManage
        public async Task<ActionResult> Index()
        {
            var packets = await db.ServicePacks.OrderBy(x => x.Fee).ToListAsync();
            return View(packets);
        }
        [HttpPost]
        public async Task<ActionResult> Checkout(ServicePack servicePack)
        {
            if (Request.Cookies["token"] == null)
                return Json(new { error = "token required" });

            string userId = userService.GetUserId(Request);
            if (userId == null)
                return Json(new { error = "invalid token" });

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["Secretkey"];

            var url = "https://localhost:44363/RoleManage";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(servicePack.Fee * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Become author"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = url + "/ConfirmCheckout/?userId=" + userId,
                CancelUrl = url + "/Index"
            };
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            Session.Add("PacketInfo", new { id = servicePack.ID, value = servicePack.PostsPerDay });

            return Redirect(session.Url);

        }

        [HttpGet]
        public async Task<ActionResult> ConfirmCheckout()
        {
            var userId = userService.GetUserId(Request);
            if (userId == null)
                return Json(new { error = "lack of information" });

            var user = await db.Users.FindAsync(userId);
            if (user == null) 
                return Json(new { error = "user not found" });

            user.RoleId = await db.Roles.Where(x => x.Name == utility.Author).Select(x => x.Id).FirstOrDefaultAsync();

            dynamic packetInfo = Session["PacketInfo"];

            var authorService = new AuthorService();
            authorService.UserId = user.Id;
            authorService.ServiceId = packetInfo.id;
            authorService.RemainPostNumber = packetInfo.value;
            db.AuthorService.Add(authorService);

            await db.SaveChangesAsync();

            var token = userService.CreateToken(user, utility.ExpiredTimeDay);
            HttpCookie cookie = new HttpCookie("token", token);
            cookie.Secure = true;
            cookie.HttpOnly = true;
            cookie.Expires = DateTime.Now.AddDays(utility.ExpiredTimeDay);
            Response.Cookies.Set(cookie);

            return RedirectToAction("Index", "Chapters", new {page=1});
        }
    }
}