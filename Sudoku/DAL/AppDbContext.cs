using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sudoku.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("SudokuConnection")
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<ChapterType> ChapterTypes { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<Discussion> Discussions { get; set; }
        public DbSet<AuthorService> AuthorService { get; set; }
        public DbSet<ServicePack> ServicePacks { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<JoinContest> JoinContests { get; set; }
        public DbSet<Rank> Ranks { get; set; }
    }
}