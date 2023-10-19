using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class SubmitDto
    {
        public string ChapterId { get; set; }
        public string SudokuString { get; set; }
        public string Type_Name { get; set; }
    }
}