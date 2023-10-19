using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class ChapterPlayDto
    {
        public Chapter chapter { set; get; }
        public string sudokuArr { set; get; }
    }
}