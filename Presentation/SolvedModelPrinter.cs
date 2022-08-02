using BusinessLogic;
using BusinessLogic.Algorithms;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;

namespace Presentation
{
    public class SolvedModelPrinter
    {
        public static void Print(Model model)
        {
            int iteration = 0;

            foreach (var table in model.Result)
            {
                Console.WriteLine($"\n\nTable {iteration}:");
                string[] headers = new string[table[0].Count];
                string temp = "";
                for (int j = 0; j < table[0].Count; j++)
                {
                    temp = table[0][j].ToString();
                    headers[j] = temp;
                }
                table.RemoveAt(0);
                var conTable = new ConsoleTable(headers);
                foreach (List<double> row in table)
                {
                    object[] rowArray = new object[row.Count];
                    for (int i = 0; i < row.Count; i++)
                    {
                        rowArray[i] = row[i];
                    }
                    conTable.AddRow(rowArray);
                }
                Console.WriteLine();
                iteration++;
                conTable.Write(Format.Alternative);
            }
        }

        public static void Print(BranchAndBoundSimplex branchAndBoundSimplex)
        {
            var tree = branchAndBoundSimplex.Results;
            PrintBranchResults(tree.Root);
            List<List<double>> bestCandidate = branchAndBoundSimplex.GetBestCandidate();

            Console.WriteLine("\n\nThis is the best solution of all the candidates:\n");
            for (int i = 0; i < bestCandidate.Count; i++)
            {
                for (int j = 0; j < bestCandidate[i].Count; j++)
                {
                    Console.Write($"\t{bestCandidate[i][j]:0.###}");
                }
                Console.WriteLine();
            }
        }

        private static void PrintBranchResults(BinaryTreeNode root, string previousProblem = "0")
        {
            if (root == null)
                return;

            if (previousProblem.Equals("0"))
            {
                Console.WriteLine("\n\n");
                Console.WriteLine("Sub-Problem 0");

                for (int i = 0; i < root.Data.Count; i++)
                {
                    for (int j = 0; j < root.Data[i].Count; j++)
                    {
                        for (int k = 0; k < root.Data[i][j].Count; k++)
                        {
                            Console.Write($"\t{root.Data[i][j][k]:0.###}");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n\n");
                }

                PrintBranchResults(root.LeftNode, "1");
                PrintBranchResults(root.RightNode, "2");
            }
            else
            {
                Console.WriteLine("\n\n");
                Console.WriteLine($"Sub-Problem {previousProblem}");

                for (int i = 0; i < root.Data.Count; i++)
                {
                    for (int j = 0; j < root.Data[i].Count; j++)
                    {
                        for (int k = 0; k < root.Data[i][j].Count; k++)
                        {
                            Console.Write($"\t{root.Data[i][j][k]:0.###}");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine("\n\n");
                }

                PrintBranchResults(root.LeftNode, previousProblem + ".1");
                PrintBranchResults(root.RightNode, previousProblem + ".2");
            }
        }
    }
}
