using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class UserSignupDto
    {
        [Required]
        public string First_name { get; set; } = string.Empty;
        [Required]
        public string Last_name { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}