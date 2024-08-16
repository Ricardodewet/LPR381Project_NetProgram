using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LP2.Program;

namespace LP2
{
    internal class PrimalSimplex
    {
        public CuttingPlane cp;
        private InputOutput inout;


        private double[,] tableau;
        private int[] basicVariables;
        private int numRows;
        private int numCols;

        public Objective ProblemObjective { get; set; }
        public VariableType VarType { get; set; }

        public PrimalSimplex(Objective objective, VariableType variableType, CuttingPlane cuttingPlane, InputOutput io)
        {
            ProblemObjective = objective;
            VarType = variableType;
            cp = cuttingPlane;
            inout = io;
        }

        public PrimalSimplex()
        {
        }



        // Method to initialize the tableau for the Simplex method
        public void InitializeTableau(double[,] inputTableau)
        {
            tableau = inputTableau;
            numRows = tableau.GetLength(0);
            numCols = tableau.GetLength(1);
            basicVariables = new int[numRows - 1];

            for (int i = 0; i < numRows - 1; i++)
            {
                basicVariables[i] = numCols - numRows + i;
            }
        }

        // Simplex Solver Implementation
        public void SimplexSolver()
        {
            while (true)
            {
                int pivotCol = 0;

                // Determine the entering variable (most negative for Max, most positive for Min)
                if (ProblemObjective == Objective.Maximize)
                {
                    double minValue = tableau[numRows - 1, 0];
                    for (int j = 1; j < numCols - 1; j++)
                    {
                        if (tableau[numRows - 1, j] < minValue)
                        {
                            minValue = tableau[numRows - 1, j];
                            pivotCol = j;
                        }
                    }

                }
                else
                {
                    double maxValue = tableau[numRows - 1, 0];
                    for (int j = 1; j < numCols - 1; j++)
                    {
                        if (tableau[numRows - 1, j] > maxValue)
                        {
                            maxValue = tableau[numRows - 1, j];
                            pivotCol = j;
                        }
                    }
                }

                // Check if optimal (no negative for Max, no positive for Min)
                if ((ProblemObjective == Objective.Maximize && tableau[numRows - 1, pivotCol] >= 0) || (ProblemObjective == Objective.Minimize && tableau[numRows - 1, pivotCol] <= 0))
                {
                    break;
                }

                int pivotRow = -1;
                double minRatio = double.MaxValue;
                for (int i = 0; i < numRows - 1; i++)
                {
                    if (tableau[i, pivotCol] > 0)
                    {
                        double ratio = tableau[i, numCols - 1] / tableau[i, pivotCol];
                        if (ratio < minRatio)
                        {
                            minRatio = ratio;
                            pivotRow = i;
                        }
                    }
                }

                // Check for unbounded solution
                if (pivotRow == -1)
                {
                    Console.WriteLine("Unbounded solution.");
                    return;
                }

                // Perform pivot operation
                Pivot(pivotRow, pivotCol);

                // Update basic variables
                basicVariables[pivotRow] = pivotCol;
            }
            Console.WriteLine("Next Pivot Table:");
            PivotTable(tableau, numRows, numCols, basicVariables);
        }

        private void Pivot(int pivotRow, int pivotCol)
        {
            double pivotElement = tableau[pivotRow, pivotCol];

            // Normalize the pivot row
            for (int j = 0; j < numCols; j++)
            {
                tableau[pivotRow, j] /= pivotElement;
            }

            // Update the rest of the tableau
            for (int i = 0; i < numRows; i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotCol];
                    for (int j = 0; j < numCols; j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }

            //InitializeTableau(tableau);
        }

        public void PivotTable(double[,] tableau, int numRows, int numCols, int[] basicVariables)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"{tableau[i, j],-10:F2} ");
                }
                Console.WriteLine();
            }
        }
    

        private void StartTableau()
        {
            Console.WriteLine("Initial Table");
            inout.DisplaySolution(tableau, numRows, numCols, basicVariables);
        }

        private bool IsIntegerSolution()
        {
            for (int i = 0; i < numRows - 1; i++)
            {
                if (Math.Abs(tableau[i, numCols - 1] - Math.Round(tableau[i, numCols - 1])) > 0)
                {
                    return false;
                }
            }
            return true;
        }


        // Main entry point for solving the problem
        public void Solve()
        {
            StartTableau();
            SimplexSolver();

            // Save the relaxed tableau before applying Cutting Plane
            double[,] savedTableau = (double[,])tableau.Clone();

            if (VarType == VariableType.Integer || VarType == VariableType.Binary)
            {
                if (!IsIntegerSolution())
                {
                    Console.WriteLine("Applying Cutting Plane Method...");
                    cp.ApplyCuttingPlane(savedTableau, numRows, numCols, basicVariables);
                }
            }
        }

    }
}