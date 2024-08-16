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

        public CuttingPlane(PrimalSimplex simplexInstance)
        {
            simplex = simplexInstance;
        }

        internal PrimalSimplex Simplex { get => simplex; set => simplex = value; }

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
                double[,] updatedTableau = AddCuttingPlane(tableau, numRows, numCols, basicVariables);

                // Debugging: Print the updated tableau
                Console.WriteLine("Updated Tableau with Cutting Plane:");
                Simplex.PivotTable(updatedTableau, numRows + 1, numCols + 1, basicVariables);

                Simplex.InitializeTableau(updatedTableau); // Ensure the Simplex instance uses the updated tableau

                // Solve with the updated tableau
                Simplex.DualSimplexSolver();

                // Check if the new solution is closer to being integer
                bool isIntegerSolution = true;
                for (int i = 0; i < numRows - 1; i++)
                {
                    double value = updatedTableau[i, numCols - 1];
                    if (Math.Abs(value - Math.Round(value)) > 0)
                    {
                        isIntegerSolution = false;
                        break;
                    }
                }

                if (isIntegerSolution)
                {
                    Console.WriteLine("Integer solution found.");
                    Simplex.PivotTable(updatedTableau, numRows + 1, numCols + 1, basicVariables);
                    break;
                }
            }
        }



        private double[,] AddCuttingPlane(double[,] tableau, int numRows, int numCols, int[] basicVariables)
        {
            int fracRow = -1;
            double minDistance = double.MaxValue;

            // Find the row with the basic variable closest to 0.5
            for (int i = 0; i < numRows - 1; i++) // Iterate through each row except the last (objective function)
            {
                double rhsValue = tableau[i, numCols - 1];
                double fractionalPart = rhsValue - Math.Floor(rhsValue);
                double distance = Math.Abs(fractionalPart - 0.5);

                // Use the lower subscript variable in case of a tie
                if (distance < minDistance ||
                   (distance == minDistance && basicVariables[i] < basicVariables[fracRow]))
                {
                    minDistance = distance;
                    fracRow = i;
                }
            }

            // Check if a valid row was found
            if (fracRow == -1)
            {
                throw new InvalidOperationException("No valid fractional row found for cutting plane.");
            }

            // Increase the size of the tableau to accommodate the new constraint
            double[,] updatedTableau = new double[numRows + 1, numCols + 1];

            // Copy the old tableau into the new one
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    updatedTableau[i, j] = tableau[i, j];
                }
            }

            // Add the new constraint (cutting plane) in the second last row
            for (int j = 0; j < numCols - 1; j++)
            {
                double fractionalPart = tableau[fracRow, j] - Math.Floor(tableau[fracRow, j]);
                updatedTableau[numRows, j] = -fractionalPart; // Add negative fractional part to the new row
            }

            // Set the RHS value of the new constraint
            double rhsFractionalPart = tableau[fracRow, numCols - 1] - Math.Floor(tableau[fracRow, numCols - 1]);
            updatedTableau[numRows, numCols - 1] = -rhsFractionalPart;

            // The new row is a basic variable for the new column
            updatedTableau[numRows, numCols] = 0; // Initial RHS value for the new constraint

            // Update the basicVariables array to include the new basic variable
            int[] newBasicVariables = new int[basicVariables.Length + 1];

            // Copy the existing basic variables into the new array
            for (int i = 0; i < basicVariables.Length; i++)
            {
                newBasicVariables[i] = basicVariables[i];
            }

            // Add the new basic variable index (the new column index) to the end of the new array
            newBasicVariables[newBasicVariables.Length - 1] = numCols - 1;

            // Update the basicVariables reference
            basicVariables = newBasicVariables;

            for (int i = 0; i < numCols; i++)
            {
                double a = updatedTableau[numRows, i];
                double b = updatedTableau[numRows - 1, i];
                updatedTableau[numRows - 1, i] = a;
                updatedTableau[numRows, i] = b;
            }

            for (int i = 0; i <= numRows; i++)
            {
                updatedTableau[i, numCols] = updatedTableau[i, numCols - 1];
                updatedTableau[i, numCols - 1] = 0;
            }

            updatedTableau[numRows - 1, numCols - 1] = 1;

            return updatedTableau; // Return the updated tableau
        }

    }

}