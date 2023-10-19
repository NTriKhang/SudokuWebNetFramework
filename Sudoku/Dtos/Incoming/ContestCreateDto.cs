using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class ContestCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Point { set; get; }
        public string Level { set; get; }
    }
}