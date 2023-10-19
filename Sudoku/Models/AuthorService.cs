using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class AuthorService
    {
        [Key]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [Required]
        public string ServiceId { get; set; }
        [ForeignKey(nameof(ServiceId))]
        public ServicePack ServicePack { get; set; }
        public int RemainPostNumber { get; set; }
    }
}