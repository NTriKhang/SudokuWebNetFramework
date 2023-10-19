using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class UserEditDto
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { set; get; }
        [Required] 
        public string Email { get; set; }
        public string ProfileImageName { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}