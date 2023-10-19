using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Rank
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        [Key]
        [Column(Order = 2)]
        public DateTime DateRank { get; set; }
        public int Value { set; get; }
        public int SubmitCount { get; set; }
    }
}