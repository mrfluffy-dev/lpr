using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Algorithms
{
    public class DualSimplex : Algorithm
    {
        public override void PutModelInCanonicalForm(Model model)
        {

            List<List<double>> tableZero = new List<List<double>>();

            tableZero.Add(new List<double>());

            foreach (var decVar in model.ObjectiveFunction.DecisionVariables)
            {
                tableZero[0].Add(decVar.Coefficient * -1);
            }


            for (int i = 0; i < model.Constraints.Count; i++)
            {
                tableZero[0].Add(0);
                if (model.Constraints[i].InequalitySign == InequalitySign.EqualTo)
                    tableZero[0].Add(0);
            }

            tableZero[0].Add(0);

            var equalsConstraints = model.Constraints.Where(c => c.InequalitySign == InequalitySign.EqualTo).ToList();
            if (equalsConstraints?.Count() > 0)
            {
                for (int i = 0; i < equalsConstraints.Count(); i++)
                {
                    model.Constraints[model.Constraints.FindIndex(c => c == equalsConstraints[i])].InequalitySign = InequalitySign.LessThanOrEqualTo;
                    var newConstraint = new Constraint();
                    newConstraint.InequalitySign = InequalitySign.GreaterThanOrEqualTo;
                    newConstraint.RightHandSide = equalsConstraints[i].RightHandSide;

                    foreach (var decVar in equalsConstraints[i].DecisionVariables)
                    {
                        newConstraint.DecisionVariables.Add(new DecisionVariable() { Coefficient = decVar.Coefficient });
                    }

                    model.Constraints.Add(newConstraint);
                }
            }

            for (int i = 0; i < model.Constraints.Count; i++)
            {
                List<double> constraintValues = new List<double>();


                foreach (var decVar in model.Constraints[i].DecisionVariables)
                {
                    if (model.Constraints[i].InequalitySign == InequalitySign.LessThanOrEqualTo)
                    {
                        constraintValues.Add(decVar.Coefficient);
                    }
                    else
                    {
                        constraintValues.Add(decVar.Coefficient * -1);
                    }
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

                if (model.Constraints[i].InequalitySign == InequalitySign.LessThanOrEqualTo)
                {
                    constraintValues.Add(model.Constraints[i].RightHandSide);
                }
                else
                {
                    constraintValues.Add(model.Constraints[i].RightHandSide * -1);
                }
                
                tableZero.Add(constraintValues);
            }

            model.Result.Add(tableZero);
        }

        public override void Solve(Model model)
        {
            Iterate(model);
            var primalSimplex = new PrimalSimplex();
            primalSimplex.Solve(model);
        }

        private void Iterate(Model model)
        {

            if (!CanPivot(model))
                return;


            int pivotRow = GetPivotRow(model);

            int pivotColumn = GetPivotColumn(model, pivotRow);

            if (pivotColumn == -1)
                throw new InfeasibleException("There is no suitable column to pivot on - the problem is infeasible");

            Pivot(model, pivotRow, pivotColumn);


            Iterate(model);
        }

        private bool CanPivot(Model model)
        {

            bool canPivot = false;
            var table = model.Result[model.Result.Count - 1];

            for (int i = 1; i < table.Count; i++)
            {

                if (table[i][table[i].Count - 1] < -0.000000000001)
                {
                    canPivot = true;
                    break;
                }
            }

            return canPivot;
        }

        private int GetPivotRow(Model model)
        {
            int pivotRow = -1;
            var table = model.Result[model.Result.Count - 1];
            double mostNegative = 0;

            for (int i = 1; i < table.Count; i++)
            {
                if (table[i][table[i].Count - 1] < 0 && table[i][table[i].Count - 1] < mostNegative)
                {
                    mostNegative = table[i][table[i].Count - 1];
                    pivotRow = i;
                }
            }

            return pivotRow;
        }

        private int GetPivotColumn(Model model, int pivotRow)
        {
            int pivotColumn = -1;
            var table = model.Result[model.Result.Count - 1];

            double lowestRatio = double.MaxValue;
            for (int i = 0; i < table[0].Count - 1; i++)
            {
                if (table[pivotRow][i] < 0)
                {
                    double ratio = Math.Abs(table[0][i] / table[pivotRow][i]);
                    if (ratio < lowestRatio)
                    {
                        lowestRatio = ratio;
                        pivotColumn = i;
                    }
                }
            }

            return pivotColumn;
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
    }
}
