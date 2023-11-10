using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class ChapterDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        public string Name { get; set; }
        public string Chap_file_path { get; set; }
        public string Type_id { get; set; }
        public ChapterType TypeNavigation { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}