using Sudoku.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Sudoku.Services
{
    public class SudokuService
    {
        private Dictionary<string, int> InitializeBox(int rows)
        {
            Dictionary<string, int> seperateBoxes = new Dictionary<string, int>();
            if (rows == 7)
            {
                //box 1
                seperateBoxes.Add("0 0", 1);
                seperateBoxes.Add("0 1", 1);
                seperateBoxes.Add("0 2", 1);
                seperateBoxes.Add("0 3", 1);
                seperateBoxes.Add("1 0", 1);
                seperateBoxes.Add("1 1", 1);
                seperateBoxes.Add("1 2", 1);
                //box 2
                seperateBoxes.Add("0 4", 2);
                seperateBoxes.Add("0 5", 2);
                seperateBoxes.Add("0 6", 2);
                seperateBoxes.Add("1 3", 2);
                seperateBoxes.Add("1 4", 2);
                seperateBoxes.Add("1 5", 2);
                seperateBoxes.Add("1 6", 2);
                //box 3
                seperateBoxes.Add("2 0", 3);
                seperateBoxes.Add("2 1", 3);
                seperateBoxes.Add("3 0", 3);
                seperateBoxes.Add("3 1", 3);
                seperateBoxes.Add("3 2", 3);
                seperateBoxes.Add("4 1", 3);
                seperateBoxes.Add("4 2", 3);
                //box 4
                seperateBoxes.Add("2 2", 4);
                seperateBoxes.Add("2 3", 4);
                seperateBoxes.Add("3 3", 4);
                seperateBoxes.Add("4 3", 4);
                seperateBoxes.Add("4 4", 4);
                seperateBoxes.Add("4 5", 4);
                seperateBoxes.Add("5 3", 4);
                //box 5
                seperateBoxes.Add("2 4", 5);
                seperateBoxes.Add("2 5", 5);
                seperateBoxes.Add("2 6", 5);
                seperateBoxes.Add("3 4", 5);
                seperateBoxes.Add("3 5", 5);
                seperateBoxes.Add("3 6", 5);
                seperateBoxes.Add("4 6", 5);
                //box 6
                seperateBoxes.Add("4 0", 6);
                seperateBoxes.Add("5 0", 6);
                seperateBoxes.Add("5 1", 6);
                seperateBoxes.Add("5 2", 6);
                seperateBoxes.Add("6 0", 6);
                seperateBoxes.Add("6 1", 6);
                seperateBoxes.Add("6 2", 6);
                //box 7
                seperateBoxes.Add("5 4", 7);
                seperateBoxes.Add("5 5", 7);
                seperateBoxes.Add("5 6", 7);
                seperateBoxes.Add("6 3", 7);
                seperateBoxes.Add("6 4", 7);
                seperateBoxes.Add("6 5", 7);
                seperateBoxes.Add("6 6", 7);

            }
            else if (rows == 5)
            {

                //box 1
                seperateBoxes.Add("0 0", 1);
                seperateBoxes.Add("0 1", 1);
                seperateBoxes.Add("0 2", 1);
                seperateBoxes.Add("1 0", 1);
                seperateBoxes.Add("1 1", 1);
                //box 2
                seperateBoxes.Add("0 3", 2);
                seperateBoxes.Add("0 4", 2);
                seperateBoxes.Add("1 3", 2);
                seperateBoxes.Add("1 4", 2);
                seperateBoxes.Add("2 4", 2);
                //box 3
                seperateBoxes.Add("2 0", 3);
                seperateBoxes.Add("3 0", 3);
                seperateBoxes.Add("3 1", 3);
                seperateBoxes.Add("4 0", 3);
                seperateBoxes.Add("4 1", 3);
                //box 4
                seperateBoxes.Add("1 2", 4);
                seperateBoxes.Add("2 1", 4);
                seperateBoxes.Add("2 2", 4);
                seperateBoxes.Add("2 3", 4);
                seperateBoxes.Add("3 2", 4);
                //box 5
                seperateBoxes.Add("3 3", 5);
                seperateBoxes.Add("3 4", 5);
                seperateBoxes.Add("4 2", 5);
                seperateBoxes.Add("4 3", 5);
                seperateBoxes.Add("4 4", 5);
            }
            else
                return null;
            return seperateBoxes;
        }
        public object TestSudoku(char[,] sudokuArr)
        {
            int rows = sudokuArr.GetLength(0), columns = sudokuArr.GetLength(1);
            var seperateBoxes = InitializeBox(rows);
            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (sudokuArr[i, j] == '0')
                        continue;
                    if (set.Contains(sudokuArr[i, j] + "in row" + i) ||
                        set.Contains(sudokuArr[i, j] + "in column" + j))
                    {
                        return new { code = 409, msg = "conflict value", row = i + 1, column = j + 1 };
                    }

                    set.Add(sudokuArr[i, j] + "in row" + i);
                    set.Add(sudokuArr[i, j] + "in column" + j);
                    if (rows == 9 && columns == 9)
                    {
                        if (set.Contains(sudokuArr[i, j] + "in box" + i / 3 + ',' + j / 3))
                        {
                            return new { code = 409, msg = "conflict value", row = i + 1, column = j + 1 };
                        }
                        else
                        {
                            set.Add(sudokuArr[i, j] + "in box" + i / 3 + ',' + j / 3);
                        }
                    }
                    else
                    {
                        int box = seperateBoxes[i + " " + j];
                        if (set.Contains(sudokuArr[i, j] + "in box" + box))
                        {
                            return new { code = 409, msg = "conflict value", row = i + 1, column = j + 1 };
                        }
                        else
                        {
                            set.Add(sudokuArr[i, j] + "in box" + box);
                        }
                    }
                }
            }
            return new { code = 200, msg = "Test conflict successfully" };
        }
        public char[,] FormatStringToArr(string sudokuString, int rows, int columns)
        {
            char[,] arr2d = new char[rows, columns];
            sudokuString = sudokuString.Remove(0, 1);
            sudokuString = sudokuString.Remove(sudokuString.Length - 1, 1);
            int i = 0, indexJ = 0, indexI = 0;
            int row = 0;
            while (row < rows)
            {
                if (sudokuString[i] == '[')
                {
                    row++;
                    int j = i + 1;
                    int col = 0;
                    while (col < columns)
                    {
                        if (char.IsDigit(sudokuString[j]))
                        {
                            col++;
                            arr2d[indexI, indexJ] = sudokuString[j];
                            indexJ++;
                            if (indexJ == columns)
                            {
                                indexI++;
                                indexJ = 0;
                            }
                        }
                        j++;
                    }
                    i = j;
                }
                i++;
            }
            return arr2d;
        }
        public string FormatArrayToString(char[,] array)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");

            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                sb.Append("[");

                for (int j = 0; j < columns; j++)
                {
                    sb.Append(array[i, j].ToString());

                    if (j < columns - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append("]");

                if (i < rows - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("]");

            return sb.ToString();
        }
        public double CaculateCompletePercent(char[,] sudokuArr)
        {

            double conflict = 0, maxPoint = 0;
            HashSet<string> visited = new HashSet<string>();
            int l1 = sudokuArr.GetLength(0), l2 = sudokuArr.GetLength(1);
            var boxes = InitializeBox(l1);

            for (int i = 0; i < l1; i++)
            {
                for (int j = 0; j < l2; j++)
                {
                    maxPoint++;

                    if (sudokuArr[i, j] == '0')
                    {
                        conflict++;
                        continue;
                    }

                    if (visited.Contains(sudokuArr[i, j] + "in row" + i))
                    {
                        conflict++;
                        continue;
                    }
                    else
                    {
                        visited.Add(sudokuArr[i, j] + "in row" + i);
                    }

                    if (visited.Contains(sudokuArr[i, j] + "in col" + j))
                    {
                        conflict++;
                        continue;
                    }
                    else
                    {
                        visited.Add(sudokuArr[i, j] + "in col" + j);
                    }

                    if (l1 == 9)
                    {
                        if (visited.Contains(sudokuArr[i, j] + "in box" + i / 3 + ' ' + j / 3))
                            conflict++;
                        else
                            visited.Add(sudokuArr[i, j] + "in box" + i / 3 + ' ' + j / 3);
                    }
                    else
                    {
                        int box = boxes[i + " " + j];
                        if (visited.Contains(sudokuArr[i, j] + "in box" + box))
                            conflict++;
                        else
                            visited.Add(sudokuArr[i, j] + "in box" + box);
                    }
                }
            }
            return 100 - (conflict * 100 / maxPoint);
        }
        public string ReadSudokuStringFromFile(string chapFileName)
        {
            string sudokuArr = "";
            var storeChapterDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Riddle", chapFileName);
            using (StreamReader reader = new StreamReader(storeChapterDirectory))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    sudokuArr += line;
                }
            }
            return sudokuArr;
        }
        public async Task WriteSudokuStringToFile(string fileName, string sudokuString)
        {
            var sudokuArr = FormatStringToArr(sudokuString, 9, 9);
            sudokuString = FormatArrayToString(sudokuArr);
            var storeChapterDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Riddle", fileName);
            using (StreamWriter streamWriter = new StreamWriter(storeChapterDirectory, false))
            {
                await streamWriter.WriteLineAsync(sudokuString);
            }
        }
        public void DeletedSudokuFile(string fileName)
        {
            var storeChapterDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot", "Riddle", fileName);
            if (System.IO.File.Exists(storeChapterDirectory))
            {
                System.IO.File.Delete(storeChapterDirectory);
            }
        }

    }
}