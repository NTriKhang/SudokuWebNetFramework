using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sudoku
{
    public static class utility
    {
        public static string Author { get; } = "Author";
        public static string Individual { get; } = "Individual";
        public static double ExpiredTimeDay { get; } = 5;
        public static string DefaultSudokuFileName { get; } = "default-chapter.txt";
        public static string StateCompleted { get; } = "Completed";
        public static string StateInComplete { get; } = "Incomplete";
        public static string LevelHard { get; } = "Hard";
        public static string LevelMedium { get; } = "Medium";
        public static string LevelEasy { get; } = "Easy";
        public static double RankTime { get; } = 10;
    }
}