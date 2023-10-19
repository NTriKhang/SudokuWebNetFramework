using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class DiscussSubmitDto
    {
        [Required]
        public string ChapterId { get; set; }
        [Required]
        public string TextContent { get; set; }

    }
}