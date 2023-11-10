using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;

namespace Sudoku.Services
{
    public class SimulatedAnnealing
    {
        private Dictionary<string, int> seperateBoxes { set; get; } = new Dictionary<string, int>();
        public SimulatedAnnealing()
        {

        }
        public Task<char[,]> getRandomRiddle(int rows)
        {
            var board = SimulatedAnnealingProcess(rows);
            Random random = new Random();
            int quality = random.Next(rows * (rows / 2), rows * 4);
            HashSet<string> indices = new HashSet<string>();
            while (quality > 0)
            {
                int i = random.Next(0, rows);
                int j = random.Next(0, rows);
                while (indices.Contains(i + " " + j))
                {
                    i = random.Next(0, rows);
                    j = random.Next(0, rows);
                }
                indices.Add(i + " " + j);
                quality--;
            }
            var res = GetRiddle(board, indices);
            return Task.FromResult(res);
        }
        private char[,] SimulatedAnnealingProcess(int rows)
        {
            Random random = new Random();
            double temperature = 1.0;
            double coolingRate = 0.999;

            var board = InitializeBoard(rows);
            int scoreBefore = CalculateScore(board);
            int cnt = 0;
            while (true)
            {
                int row = random.Next(0, rows);
                int col = random.Next(0, rows);

                int nextRow = 0, nextCol = 0;
                GenerateNextIndex(rows, row, col, ref nextRow, ref nextCol);

                if (scoreBefore == 0)
                {
                    return board;
                }

                char tmp = board[row, col];
                board[row, col] = board[nextRow, nextCol];
                board[nextRow, nextCol] = tmp;

                int scoreAfter = CalculateScore(board);

                double delta = scoreAfter - scoreBefore;

                double compareNumber = Math.Exp(-delta / temperature);
                double acceptNumber = random.NextDouble();

                if (delta <= 0 || compareNumber > acceptNumber)
                {
                    if (scoreBefore == scoreAfter)
                    {
                        cnt++;
                        if (cnt > 500)
                        {
                            temperature = 1.0;
                            cnt = 0;
                        }
                    }
                    else
                    {
                        cnt = 0;
                    }
                    scoreBefore = scoreAfter;
                    continue;
                }
                else
                {
                    tmp = board[row, col];
                    board[row, col] = board[nextRow, nextCol];
                    board[nextRow, nextCol] = tmp;
                }


                temperature *= coolingRate;
            }
        }
        private char[,] InitializeBoard(int rows)
        {
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

            if (rows == 9)
            {
                var board = new char[9, 9]
                {
                    { '5', '3', '4', '6', '7', '5', '9', '1', '2' },
                    { '6', '2', '7', '1', '9', '8', '3', '4', '8' },
                    { '1', '9', '8', '3', '4', '2', '6', '5', '7' },
                    { '8', '5', '9', '7', '6', '1', '4', '2', '3' },
                    { '4', '2', '6', '8', '5', '3', '7', '9', '1' },
                    { '7', '1', '3', '9', '2', '4', '8', '5', '6' },
                    { '3', '6', '1', '3', '5', '7', '2', '8', '4' },
                    { '2', '7', '8', '9', '4', '1', '6', '3', '5' },
                    { '9', '4', '5', '2', '8', '6', '7', '9', '1' }
                };
                return board;
            }
            else if (rows == 7)
            {
                var board = new char[7, 7]
                {
                    { '5', '1', '2', '7', '3', '5', '7' },
                    { '6', '4', '3', '1', '2', '4', '6' },
                    { '1', '2', '1', '2', '1', '3', '7' },
                    { '3', '4', '5', '3', '2', '4', '6' },
                    { '2', '7', '6', '4', '5', '6', '5' },
                    { '1', '4', '6', '7', '3', '4', '6' },
                    { '3', '5', '7', '1', '2', '5', '7' }
                };
                return board;
            }
            else
            {
                char[,] board = new char[5, 5]
                {
                    { '1', '3', '4', '3', '1' },
                    { '2', '5', '4', '2', '5' },
                    { '1', '1', '2', '5', '4' },
                    { '3', '5', '3', '3', '4' },
                    { '4', '2', '1', '2', '5' }
                };
                return board;
            }
        }
        private int CalculateScore(char[,] board)
        {
            int score = 0;

            for (int i = 0; i < board.GetLength(0); i++)
            {
                HashSet<char> set = new HashSet<char>();
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (set.Contains(board[i, j]))
                        score++;
                    else
                        set.Add(board[i, j]);
                }
            }
            for (int i = 0; i < board.GetLength(0); i++)
            {
                HashSet<char> set = new HashSet<char>();
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (set.Contains(board[j, i]))
                        score++;
                    else
                        set.Add(board[j, i]);
                }
            }
            return score;
        }
        private void GenerateNextIndex(int rows, int row, int col, ref int nextRow, ref int nextCol)
        {
            Random random = new Random();
            if (rows == 9)
            {
                if (row >= 6)
                    nextRow = random.Next(6, 9);
                else if (row >= 3)
                    nextRow = random.Next(3, 6);
                else
                    nextRow = random.Next(0, 3);

                if (col >= 6)
                    nextCol = random.Next(6, 9);
                else if (col >= 3)
                    nextCol = random.Next(3, 6);
                else
                    nextCol = random.Next(0, 3);
            }
            else
            {
                do
                {
                    nextRow = random.Next(0, rows);
                    nextCol = random.Next(0, rows);
                } while (seperateBoxes[row + " " + col] != seperateBoxes[nextRow + " " + nextCol]);
            }
        }
        private char[,] GetRiddle(char[,] board, HashSet<string> indices)
        {
            char[,] res = new char[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (indices.Contains(i + " " + j))
                    {
                        res[i, j] = board[i, j];
                    }
                    else
                    {
                        res[i, j] = '0';
                    }
                }

            }
            return res;
        }
    }
}