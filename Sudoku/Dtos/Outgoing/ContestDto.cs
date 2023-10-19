using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Dtos.Outgoing
{
    public class ContestDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Point { set; get; }
        public int NumberOfJoined { set; get; } = 0;
        public string Level { set; get; }
    }
}