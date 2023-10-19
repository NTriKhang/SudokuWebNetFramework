using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class SubmissionDetailDto
    {
        public string State { get; set; }
        public double CompletedPercent { get; set; }
        public DateTime SubmitDate { get; set; }
        public string ChapterName { get; set; }
        public string SudokuString { get; set; }
        public string NameType { get; set; }
    }
}