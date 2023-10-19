using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{

    public class JoinContest
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 6);
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string ContestId { get; set; }
        [ForeignKey("ContestId")]
        public Contest Contest { get; set; }
        public bool State { set; get; } // win or lose 
    }
}