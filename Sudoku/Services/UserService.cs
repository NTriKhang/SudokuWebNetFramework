using Microsoft.Ajax.Utilities;
using Microsoft.IdentityModel.Tokens;
using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Sudoku.Services
{
    public class UserService
    {
        public string GetUserId(HttpRequestBase Request)
        {
            try
            {
                if (Request.Cookies["token"] == null)
                    return "";
                var token = Request.Cookies["token"].Value;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var securityToken = handler.ReadJwtToken(token);
                var userId = securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                return userId;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public string GetUserRole(HttpRequestBase Request)
        {
            if (Request.Cookies["token"] == null)
                return "";
            var token = Request.Cookies["token"].Value;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var securityToken = handler.ReadJwtToken(token);
            var userId = securityToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            return userId;
        }
        public string CreateToken(User user, double expired_time)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.RoleId)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("this is my top jwt secret key for authentication and i append it to have enough lenght"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(expired_time),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}