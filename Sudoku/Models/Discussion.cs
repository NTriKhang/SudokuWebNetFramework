using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Discussion
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string ChapterId { get; set; }
        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter { get; set; }
    }
}