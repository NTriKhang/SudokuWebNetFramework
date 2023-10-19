using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Chapter
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        public string Name { get; set; }
        public string Chap_file_path { get; set; }
        public string Type_id { get; set; }
        [ForeignKey(nameof(Type_id))]
        public ChapterType TypeNavigation { get; set; }
        public DateTime CreateDate { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}