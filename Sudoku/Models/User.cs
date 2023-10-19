using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Email_confirm { get; set; }
        public string RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public virtual Role RoleNavigation { get; set; }
        public string Profile_Image { get; set; } = string.Empty;
        public int Point { set; get; }
    }
}