using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sudoku.Dtos.Outgoing
{
    public class ChapterUpsertDto
    {
        public string ChapterId { get; set; }
        public string Name { get; set; }
        public string Chap_file_name { get; set; }
        public string Type_id { get; set; }
        public string SudokuString { get; set; }
        public int RemainPost { get; set; }

        public IEnumerable<SelectListItem> ChapterType { get; set; }
    }
}