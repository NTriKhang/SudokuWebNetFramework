using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sudoku.DAL;
using Sudoku.Dtos.Incoming;
using Sudoku.Dtos.Outgoing;
using Sudoku.Models;
using Sudoku.Services;

namespace Sudoku.Controllers
{
    public class UsersController : Controller
    {
        private AppDbContext db = new AppDbContext();
        private UserService userService = new UserService();
        private static int expired_time = 5;

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> SignUp(UserSignupDto userSignup, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
                return View(userSignup);
            try
            {
                bool isEmailExits = await db.Users.AnyAsync(x => x.Email == userSignup.Email);
                if (isEmailExits)
                {
                    return Json(new { msg = "email have already exits", code = 409 });
                }

                string imgName = "";
                if (file == null)
                {
                    imgName = "UserNoImage.jpg";
                }
                else
                {
                    imgName = Guid.NewGuid().ToString().Substring(0, 6) + file.FileName;
                    string imagePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "ProfileImage", imgName);
                    file.SaveAs(imagePath);
                }

                var user = new User()
                {
                    First_name = userSignup.First_name,
                    Last_name = userSignup.Last_name,
                    Email = userSignup.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(userSignup.Password),
                    Email_confirm = false,
                    RoleId = await db.Roles.Where(x => x.Name == utility.Individual).Select(x => x.Id).FirstOrDefaultAsync(),
                    Point = 0,
                    Profile_Image = imgName,
                };

                var res = db.Users.Add(user);

                int num = await db.SaveChangesAsync();
                if (num == 0)
                {
                    throw new Exception("Nothing was saved to database");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserLoginDto userLogin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await db.Users.Where(x => x.Email == userLogin.Email).FirstOrDefaultAsync();
                    if (user == null)
                    {
                        TempData["ErrorMessage"] = "Incorrect email";
                        return View(userLogin);
                    }

                    if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
                    {
                        TempData["ErrorMessage"] = "Incorrect password";
                        return View(userLogin);
                    }

                    string token = userService.CreateToken(user, expired_time);
                    HttpCookie httpCookie = new HttpCookie("token", token);
                    httpCookie.HttpOnly = true;
                    httpCookie.Secure = true;
                    httpCookie.Expires = DateTime.Now.AddDays(expired_time);
                    Response.Cookies.Set(httpCookie);

                    return RedirectToAction("Index", "Home");
                }
                return View(userLogin);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                HttpCookie httpCookie = Request.Cookies["token"];
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Set(httpCookie);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                throw;
            }
        }
        //[NonAction]
        //public async Task<string> CreateToken(User user)
        //{
        //    string role = "";
        //    if (user.Email == "nguyentrikhang1703@gmail.com")
        //        role = await db.Roles.Where(x => x.Name == "Author").Select(x => x.Id).FirstOrDefaultAsync();
        //    else
        //        role = await db.Roles.Where(x => x.Name == "Individual").Select(x => x.Id).FirstOrDefaultAsync();
        //    List<Claim> claims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id),
        //        new Claim(ClaimTypes.Role, role)
        //    };

        //    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my top jwt secret key for authentication and i append it to have enough lenght"));
        //    var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
        //    var token = new JwtSecurityToken(
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(expired_time),
        //        signingCredentials: cred
        //        );
        //    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        //    return jwt;
        //}
        [HttpGet]
        public bool isLogin()
        {
            if (Request.Cookies["token"] != null)
                return true;
            return false;
        }
        [HttpGet]
        public async Task<ActionResult> UserProfile()
        {
            if (Request.Cookies["token"] == null)
            {
                return Json(new { code = 400, msg = "Bad request" });
            }
            var userId = userService.GetUserId(Request);
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);

            UserDto userDto = new UserDto()
            {
                First_name = user.First_name,
                Last_name = user.Last_name,
                Email = user.Email,
                Id = user.Id,
                Point = user.Point,
                Profile_Image = "\\" + Path.Combine("wwwroot", "ProfileImage", user.Profile_Image)
            };
            return View(userDto);
        }
        [HttpGet]
        public async Task<ActionResult> User()
        {
            if (Request.Cookies["token"] == null)
            {
                return Json( null );
            }
            var userId = userService.GetUserId(Request);
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userId);

            UserDto userDto = new UserDto()
            {
                Point = user.Point,
                Profile_Image = "\\" + Path.Combine("wwwroot", "ProfileImage", user.Profile_Image)
            };
            return Json(userDto, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> EditProfile()
        {
            var userId = userService.GetUserId(Request);
            var user = await db.Users.FindAsync(userId);
            if (user == null)
                return HttpNotFound();

            var userEdit = new UserEditDto()
            {
                Id = user.Id,
                FirstName = user.First_name,
                LastName = user.Last_name,
                Email = user.Email,
                ProfileImageName = user.Profile_Image,
                ProfileImageUrl = '/' + "wwwroot/ProfileImage/" + user.Profile_Image
            };
            return View(userEdit);
        }

        [HttpPost]
        public async Task<ActionResult> EditProfile(UserEditDto userEdit, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return Json(new {msg = "lack of information", code = 409});
            }
            try
            {
                var user = await db.Users.FindAsync(userEdit.Id);
                if (user.Email != userEdit.Email)
                {
                    bool isEmailExits = await db.Users.AnyAsync(x => x.Email == user.Email);
                    if (isEmailExits)
                    {
                        return Json(new { msg = "email have already exits", code = 409 });
                    }
                }

                string imgName = "";
                if (file == null)
                {
                    imgName = user.Profile_Image;
                }
                else
                {
                    imgName = Guid.NewGuid().ToString().Substring(0, 6) + file.FileName;
                    string imagePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "ProfileImage", imgName);
                    string oldPath = Path.Combine(AppContext.BaseDirectory, "wwwroor", "ProfileImage", user.Profile_Image);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    file.SaveAs(imagePath);
                }

                user.First_name = userEdit.FirstName;
                user.Last_name = userEdit.LastName;
                user.Email = userEdit.Email;
                user.Profile_Image = imgName;

                await db.SaveChangesAsync();
                return Json(new { code = 200 });
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

    }
}
