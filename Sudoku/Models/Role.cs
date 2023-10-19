using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.Models
{
    public class Role
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString().Substring(0, 6);
            Name = "";
        }
        public Role(string Name)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 6);
            this.Name = Name;
        }
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}