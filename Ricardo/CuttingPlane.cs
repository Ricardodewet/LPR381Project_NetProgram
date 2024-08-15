using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LP2
{
    internal class CuttingPlane
    {

        private PrimalSimplex simplex;

        internal PrimalSimplex Simplex { get => simplex; set => simplex = value; }

        // Cutting Plane Method Implementation
        public void ApplyCuttingPlane(double[,] tableau, int numRows, int numCols, int[] basicVariables)
        {
            while (true)
            {
                int fracRow = -1;
                double fracValue = 0.0;

                for (int i = 0; i < numRows - 1; i++)
                {
                    double value = tableau[i, numCols - 1];
                    if (Math.Abs(value - Math.Round(value)) > 0)
                    {
                        fracRow = i;
                        fracValue = value - Math.Floor(value);
                        break;
                    }
                }

                if (fracRow == -1)
                {
                    Console.WriteLine("Integer solution found.");
                    Simplex.PivotTable(tableau, numRows, numCols, basicVariables);
                    break;
                }

                // Add a cutting plane constraint
                AddCuttingPlane(tableau, numRows, numCols, fracRow, fracValue);
                Simplex.SimplexSolver();
            }
        }

        private void AddCuttingPlane(double[,] tableau, int numRows, int numCols, int fracRow, double fracValue)
        {
            // Increase the size of the tableau to accommodate the new constraint
            numRows++;
            numCols++;

            double[,] newTableau = new double[numRows, numCols];

            // Copy the old tableau into the new one, ensuring the RHS is updated correctly
            for (int i = 0; i < numRows - 1; i++)
            {
                for (int j = 0; j < numCols - 2; j++)  // Copy all columns except the last one (RHS)
                {
                    newTableau[i, j] = tableau[i, j];
                }
                // Copy the RHS column values from the previous tableau's RHS column
                newTableau[i, numCols - 1] = tableau[i, numCols - 2];
            }

            // Add the new constraint (cutting plane)
            for (int j = 0; j < numCols - 2; j++)
            {
                newTableau[numRows - 1, j] = Math.Floor(tableau[fracRow, j]) - tableau[fracRow, j];
            }

            // Set the new column values (for the new slack variable)
            for (int i = 0; i < numRows - 1; i++)
            {
                newTableau[i, numCols - 2] = 0;  // New slack variable column (second last column)
            }
            newTableau[numRows - 1, numCols - 2] = 1;  // New slack variable for the new constraint

            // Set the right-hand side (RHS) value for the new constraint
            double newRHSValue = Math.Floor(tableau[fracRow, numCols - 2]) - tableau[fracRow, numCols - 2];
            newTableau[numRows - 1, numCols - 1] = newRHSValue;

            // Update the tableau reference
            tableau = newTableau;

            // Ensure the basic variables are set correctly
            Simplex.InitializeTableau(tableau);
        }
    }
}
