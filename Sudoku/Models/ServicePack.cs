using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class ServicePack
    {
        public ServicePack()
        {

        }
        public ServicePack(double fee, int postsPerDay)
        {
            Fee = fee;
            PostsPerDay = postsPerDay;
        }
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        [Required]
        public double Fee { get; set; }
        [Required]
        public int PostsPerDay { get; set; }
    }
}