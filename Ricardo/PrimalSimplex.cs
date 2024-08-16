//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static LP2.Program;

//namespace LP2
//{
//    internal class PrimalSimplex
//    {
//        public CuttingPlane cp;
//        private InputOutput inout;

//        private double[,] tableau;
//        private int[] basicVariables;
//        private int numRows;
//        private int numCols;

//        public Objective ProblemObjective { get; set; }
//        public VariableType VarType { get; set; }

//        public PrimalSimplex(Objective objective, VariableType variableType, CuttingPlane cuttingPlane, InputOutput io)
//        {
//            ProblemObjective = objective;
//            VarType = variableType;
//            cp = cuttingPlane;
//            inout = io;
//        }

//        public PrimalSimplex()
//        {
//        }

//        public void InitializeTableau(double[,] inputTableau)
//        {
//            tableau = inputTableau;
//            numRows = tableau.GetLength(0);
//            numCols = tableau.GetLength(1);
//            basicVariables = new int[numRows - 1];

//            for (int i = 0; i < numRows - 1; i++)
//            {
//                basicVariables[i] = numCols - numRows + i;
//            }
//        }

//        public void SimplexSolver()
//        {
//            bool dualSimplex = HasNegativeRHS();
//            bool solved = false;

//            while (!solved)
//            {
//                if (dualSimplex)
//                {
//                    Console.WriteLine("Applying Dual Simplex Method...");
//                    DualSimplexSolver();
//                    dualSimplex = false; // Assuming we only need to apply Dual Simplex once
//                }
//                else
//                {
//                    int pivotCol = GetPivotColumn();
//                    if (IsOptimal(pivotCol))
//                    {
//                        solved = true;
//                    }
//                    else
//                    {
//                        int pivotRow = GetPivotRow(pivotCol);
//                        if (pivotRow == -1)
//                        {
//                            Console.WriteLine("Unbounded solution.");
//                            return;
//                        }

//                        Pivot(pivotRow, pivotCol);
//                        basicVariables[pivotRow] = pivotCol;
//                    }
//                }
//            }

//            Console.WriteLine("Next Pivot Table:");
//            PivotTable(tableau, numRows, numCols, basicVariables);

//            double[,] savedTableau = (double[,])tableau.Clone();

//            if (VarType == VariableType.Integer || VarType == VariableType.Binary)
//            {
//                if (!IsIntegerSolution())
//                {
//                    Console.WriteLine("Applying Cutting Plane Method...");
//                    cp.ApplyCuttingPlane(savedTableau, numRows, numCols, basicVariables);
//                }
//            }
//        }

//        private int GetPivotColumn()
//        {
//            int pivotCol = 0;

//            if (ProblemObjective == Objective.Maximize)
//            {
//                double minValue = tableau[numRows - 1, 0];
//                for (int j = 1; j < numCols - 1; j++)
//                {
//                    if (tableau[numRows - 1, j] < minValue)
//                    {
//                        minValue = tableau[numRows - 1, j];
//                        pivotCol = j;
//                    }
//                }
//            }
//            else
//            {
//                double maxValue = tableau[numRows - 1, 0];
//                for (int j = 1; j < numCols - 1; j++)
//                {
//                    if (tableau[numRows - 1, j] > maxValue)
//                    {
//                        maxValue = tableau[numRows - 1, j];
//                        pivotCol = j;
//                    }
//                }
//            }
//            return pivotCol;
//        }

//        private bool IsOptimal(int pivotCol)
//        {
//            return (ProblemObjective == Objective.Maximize && tableau[numRows - 1, pivotCol] >= 0) ||
//                   (ProblemObjective == Objective.Minimize && tableau[numRows - 1, pivotCol] <= 0);
//        }

//        private int GetPivotRow(int pivotCol)
//        {
//            int pivotRow = -1;
//            double minRatio = double.MaxValue;

//            for (int i = 0; i < numRows - 1; i++)
//            {
//                if (tableau[i, pivotCol] > 0)
//                {
//                    double ratio = tableau[i, numCols - 1] / tableau[i, pivotCol];
//                    if (ratio < minRatio)
//                    {
//                        minRatio = ratio;
//                        pivotRow = i;
//                    }
//                }
//            }
//            return pivotRow;
//        }

//        private void Pivot(int pivotRow, int pivotCol)
//        {
//            double pivotElement = tableau[pivotRow, pivotCol];

//            for (int j = 0; j < numCols; j++)
//            {
//                tableau[pivotRow, j] /= pivotElement;
//            }

//            for (int i = 0; i < numRows; i++)
//            {
//                if (i != pivotRow)
//                {
//                    double factor = tableau[i, pivotCol];
//                    for (int j = 0; j < numCols; j++)
//                    {
//                        tableau[i, j] -= factor * tableau[pivotRow, j];
//                    }
//                }
//            }
//        }

//        public void PivotTable(double[,] tableau, int numRows, int numCols, int[] basicVariables)
//        {
//            for (int i = 0; i < numRows; i++)
//            {
//                for (int j = 0; j < numCols; j++)
//                {
//                    Console.Write($"{tableau[i, j],-10:F2} ");
//                }
//                Console.WriteLine();
//            }
//        }

//        private void StartTableau()
//        {
//            Console.WriteLine("Initial Table");
//            inout.DisplaySolution(tableau, numRows, numCols, basicVariables);
//        }

//        private bool IsIntegerSolution()
//        {
//            for (int i = 0; i < numRows - 1; i++)
//            {
//                if (Math.Abs(tableau[i, numCols - 1] - Math.Round(tableau[i, numCols - 1])) > 0)
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        private bool HasNegativeRHS()
//        {
//            for (int i = 0; i < numRows - 1; i++)
//            {
//                if (tableau[i, numCols - 1] < 0)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        private void DualSimplexSolver()
//        {
//            while (true)
//            {
//                int pivotRow = 0;

//                for (int i = 0; i < numRows - 1; i++)
//                {
//                    if (tableau[i, numCols - 1] < 0)
//                    {
//                        pivotRow = i;
//                        break;
//                    }
//                }

//                int pivotCol = -1;
//                double maxRatio = double.MinValue;
//                for (int j = 0; j < numCols - 1; j++)
//                {
//                    if (tableau[pivotRow, j] < 0)
//                    {
//                        double ratio = -tableau[numRows - 1, j] / tableau[pivotRow, j];
//                        if (ratio > maxRatio)
//                        {
//                            maxRatio = ratio;
//                            pivotCol = j;
//                        }
//                    }
//                }

//                if (pivotCol == -1)
//                {
//                    Console.WriteLine("No feasible solution.");
//                    return;
//                }

//                Pivot(pivotRow, pivotCol);
//            }
//            Console.WriteLine("Next Pivot Table:");
//            PivotTable(tableau, numRows, numCols, basicVariables);
//        }

//        public void Solve()
//        {
//            StartTableau();
//            if (HasNegativeRHS())
//            {
//                Console.WriteLine("Applying Dual Simplex Method...");
//                DualSimplexSolver();
//            }
//            else
//            {
//                SimplexSolver();
//            }

//            double[,] savedTableau = (double[,])tableau.Clone();

//            if (VarType == VariableType.Integer || VarType == VariableType.Binary)
//            {
//                if (!IsIntegerSolution())
//                {
//                    Console.WriteLine("Applying Cutting Plane Method...");
//                    cp.ApplyCuttingPlane(savedTableau, numRows, numCols, basicVariables);
//                }
//            }
//        }
//    }


//}

using System;
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

        public void Solve()
        {
            StartTableau();
            if (HasNegativeRHS())
            {
                Console.WriteLine("Applying Dual Simplex Method...");
                DualSimplexSolver();
            }
            else
            {
                Console.WriteLine("Applying Primal Simplex Method...");
                SimplexSolver();
            }

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

        private bool HasNegativeRHS()
        {
            for (int i = 0; i < numRows - 1; i++)
            {
                if (tableau[i, numCols - 1] < 0)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetNegativeRHSRow()
        {
            int pivotRow = -1;
            double minValue = 0;

            for (int i = 0; i < numRows - 1; i++)
            {
                if (tableau[i, numCols - 1] < minValue)
                {
                    minValue = tableau[i, numCols - 1];
                    pivotRow = i;
                }
            }
            return pivotRow;
        }

        private int GetPivotColumnForDual(int pivotRow)
        {
            int pivotCol = -1;
            double maxRatio = double.MinValue;

            for (int j = 0; j < numCols - 1; j++)
            {
                if (tableau[pivotRow, j] < 0)
                {
                    double ratio = -tableau[numRows - 1, j] / tableau[pivotRow, j];
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                        pivotCol = j;
                    }
                }
            }
            return pivotCol;
        }

        public void DualSimplexSolver()
        {
            bool continueLoop = true;

            while (continueLoop)
            {
                int pivotRow = GetNegativeRHSRow();
                if (pivotRow == -1)
                {
                    Console.WriteLine("No negative RHS found; exiting Dual Simplex.");
                    break;
                }

                int pivotCol = GetPivotColumnForDual(pivotRow);
                if (pivotCol == -1)
                {
                    Console.WriteLine("No valid pivot column found; exiting Dual Simplex.");
                    break;
                }

                Pivot(pivotRow, pivotCol);
                Console.WriteLine("Updated Tableau after Dual Simplex pivot:");
                PivotTable(tableau, numRows, numCols, basicVariables);

                // Check if there's still a negative RHS
                continueLoop = HasNegativeRHS();
            }
            Console.WriteLine("done");
        }

        public void SimplexSolver()
        {
            bool solved = false;

            while (!solved)
            {
                int pivotCol = GetPivotColumn();
                if (IsOptimal(pivotCol))
                {
                    solved = true;
                }
                else
                {
                    int pivotRow = GetPivotRow(pivotCol);
                    if (pivotRow == -1)
                    {
                        Console.WriteLine("Unbounded solution.");
                        return;
                    }

                    Pivot(pivotRow, pivotCol);
                    basicVariables[pivotRow] = pivotCol;
                }
            }

            Console.WriteLine("Next Pivot Table:");
            PivotTable(tableau, numRows, numCols, basicVariables);
        }

        private int GetPivotColumn()
        {
            int pivotCol = 0;

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
            return pivotCol;
        }

        private bool IsOptimal(int pivotCol)
        {
            return (ProblemObjective == Objective.Maximize && tableau[numRows - 1, pivotCol] >= 0) ||
                   (ProblemObjective == Objective.Minimize && tableau[numRows - 1, pivotCol] <= 0);
        }

        private int GetPivotRow(int pivotCol)
        {
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
            return pivotRow;
        }

        private void Pivot(int pivotRow, int pivotCol)
        {
            double pivotElement = tableau[pivotRow, pivotCol];

            for (int j = 0; j < numCols; j++)
            {
                tableau[pivotRow, j] /= pivotElement;
            }

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
    }
}

