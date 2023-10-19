using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Profile_Image { get; set; } = string.Empty;
        public int Point { set; get; }
    }
}