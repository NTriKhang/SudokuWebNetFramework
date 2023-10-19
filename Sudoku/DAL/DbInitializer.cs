using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku.DAL
{
    public class DbInitializer : System.Data.Entity.CreateDatabaseIfNotExists<AppDbContext>
    {
        protected override void Seed(AppDbContext context)
        {
            var roles = new List<Role>()
            {
                new Role("Individual"),
                new Role("Author")
            };
            context.Roles.AddRange(roles);

            var chapterTypes = new List<ChapterType>()
            {
                new ChapterType("Easy", 25),
                new ChapterType("Medium", 50),
                new ChapterType("Hard", 75),
            };
            context.ChapterTypes.AddRange(chapterTypes);

            context.SaveChanges();

        }
    }
}
