using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class ChapterType
    {
        public ChapterType()
        {

        }
        public ChapterType(string Name, int Point)
        {
            this.Name = Name;
            this.Point = Point;
        }
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        public string Name { get; set; }
        public int Point { set; get; }
    }
}