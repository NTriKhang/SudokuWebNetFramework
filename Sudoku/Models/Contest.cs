using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Contest
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 7);
        public string Name { get; set; }
        public int Point { set; get; }
        public DateTime DateTime { set; get; }
        public string Level { set; get; }
    }
}