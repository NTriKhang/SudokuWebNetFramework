using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Submission
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString().Substring(0, 6);
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [Required]
        public string ChapterId { get; set; }
        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter { get; set; }
        public string State { get; set; }
        public double CompletedPercent { get; set; }
        public DateTime SubmitDate { get; set; }
        public string Submit_path_file { get; set; }
    }
}