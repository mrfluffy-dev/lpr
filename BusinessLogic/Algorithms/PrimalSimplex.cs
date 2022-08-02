using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Algorithms
{
    public class PrimalSimplex : Algorithm
    {
        public override void PutModelInCanonicalForm(Model model)
        {

            List<List<double>> tableZero = new List<List<double>>();

 
            tableZero.Add(new List<double>());

            foreach (var decVar in model.ObjectiveFunction.DecisionVariables)
            {
                tableZero[0].Add(decVar.Coefficient * -1);
            }


            for (int i = 0; i <= model.Constraints.Count; i++)
            {
                tableZero[0].Add(0);
            }

            for (int i = 0; i < model.Constraints.Count; i++)
            {
                List<double> constraintValues = new List<double>();

                foreach (var decVar in model.Constraints[i].DecisionVariables)
                {
                    constraintValues.Add(decVar.Coefficient);
                }

                for (int j = 0; j < model.Constraints.Count; j++)
                {
                    if (j == i)
                    {
                        constraintValues.Add(1);
                    }
                    else
                    {
                        constraintValues.Add(0);
                    }
                }

                constraintValues.Add(model.Constraints[i].RightHandSide);

                tableZero.Add(constraintValues);
            }

            model.Result.Add(tableZero);
        }

        public override void Solve(Model model)
        {
            Iterate(model);
        }

        private bool IsOptimal(Model model)
        {
            bool isOptimal = true;
            var table = model.Result[model.Result.Count - 1];

            if (model.ProblemType == ProblemType.Maximization)
            {
                for (int i = 0; i < table[0].Count - 1; i++)
                {
                    if (table[0][i] < 0)
                    {
                        isOptimal = false;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < table[0].Count - 1; i++)
                {
                    if (table[0][i] > 0)
                    {
                        isOptimal = false;
                        break;
                    }
                }
            }

            return isOptimal;
        }

        private void Iterate(Model model)
        {

            if (IsOptimal(model))
                return;


            int pivotColumn = GetPivotColumn(model);
            // Then get the pivot row
            int pivotRow = GetPivotRow(model, pivotColumn);

            if (pivotRow == -1)
                throw new InfeasibleException("There is no suitable row to pivot on - the problem is infeasible");

            Pivot(model, pivotRow, pivotColumn);

            Iterate(model);
        }

        private void Pivot(Model model, int pivotRow, int pivotColumn)
        {
            var previousTable = model.Result[model.Result.Count - 1];
            var newTable = new List<List<double>>();

            for (int i = 0; i < previousTable.Count; i++)
            {
                newTable.Add(new List<double>());

                for (int j = 0; j < previousTable[i].Count; j++)
                {
                    newTable[i].Add(previousTable[i][j]);
                }
            }


            double factor = 1 / newTable[pivotRow][pivotColumn];
            for (int i = 0; i < newTable[pivotRow].Count; i++)
            {
                newTable[pivotRow][i] *= factor;
            }

            double pivotColumnValue;
            for (int i = 0; i < newTable.Count; i++)
            {
                pivotColumnValue = newTable[i][pivotColumn];

                if (i != pivotRow && pivotColumnValue != 0)
                {
                    for (int j = 0; j < newTable[i].Count; j++)
                    {
                        newTable[i][j] += (-1 * pivotColumnValue * newTable[pivotRow][j]);
                    }
                }
            }

            model.Result.Add(newTable);
        }

        private int GetPivotColumn(Model model)
        {
            int colIndex = -1;
            var table = model.Result[model.Result.Count - 1];

            if (model.ProblemType == ProblemType.Maximization)
            {
                double mostNegative = 0;

                for (int i = 0; i < table[0].Count - 1; i++)
                {
                    if (table[0][i] < 0 && table[0][i] < mostNegative)
                    {
                        mostNegative = table[0][i];
                        colIndex = i;
                    }
                }
            }
            else
            {
                double mostPositive = 0;

                for (int i = 0; i < table[0].Count - 1; i++)
                {
                    if (table[0][i] > 0 && table[0][i] > mostPositive)
                    {
                        mostPositive = table[0][i];
                        colIndex = i;
                    }
                }
            }

            return colIndex;
        }

        private int GetPivotRow(Model model, int pivotColumn)
        {
            int rowIndex = -1;
            var table = model.Result[model.Result.Count - 1];

            double lowestRatio = double.MaxValue;
            for (int i = 1; i < table.Count; i++)
            {
                if (table[i][pivotColumn] > 0)
                {
                    double ratio = table[i][table[i].Count - 1] / table[i][pivotColumn];
                    if (ratio < lowestRatio && ratio >= 0)
                    {
                        lowestRatio = ratio;
                        rowIndex = i;
                    }
                }
            }

            return rowIndex;
        }
    }
}
