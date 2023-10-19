using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Incoming
{
    public class DiscussDeleteDto
    {
        public string chapterId { set; get; }
        public string discussId { set; get; }
    }
}