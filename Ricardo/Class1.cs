using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LP2
{
    internal class CuttingPlane1
    {

        private PrimalSimplex simplex;

        public CuttingPlane1(PrimalSimplex simplexInstance)
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
                AddCuttingPlane1(tableau, numRows, numCols, fracRow, fracValue);
                Simplex.SimplexSolver();

                // Check if the new solution is closer to being integer
                bool isIntegerSolution = true;
                for (int i = 0; i < numRows - 1; i++)
                {
                    double value = tableau[i, numCols - 1];
                    if (Math.Abs(value - Math.Round(value)) > 0)
                    {
                        isIntegerSolution = false;
                        break;
                    }
                }

                if (isIntegerSolution)
                {
                    Console.WriteLine("Integer solution found.");
                    Simplex.PivotTable(tableau, numRows, numCols, basicVariables);
                    break;
                }
            }
        }




        // Add constraint op regte column maar nir die ry nie.
        private void AddCuttingPlane1(double[,] tableau, int numRows, int numCols, int fracRow, double fracValue)
        {
            //Increase the size of the tableau to accommodate the new constraint
            numRows++;
            numCols++;

            double[,] newTableau = new double[numRows, numCols];

            //Copy the old tableau into the new one, ensuring the RHS is updated correctly
            for (int i = 0; i < numRows - 1; i++)
            {
                for (int j = 0; j < numCols - 2; j++)  // Copy all columns except the last one (RHS)
                {
                    newTableau[i, j] = tableau[i, j];
                }
                //Copy the RHS column values from the previous tableau's RHS column
                newTableau[i, numCols - 1] = tableau[i, numCols - 2];
            }

            //Add the new constraint(cutting plane)
            for (int j = 0; j < numCols - 2; j++)
            {
                newTableau[numRows - 1, j] = Math.Floor(tableau[fracRow, j]) - tableau[fracRow, j];
            }

            //Set the new column values(for the new slack variable)
                for (int i = 0; i < numRows - 1; i++)
                {
                    newTableau[i, numCols - 2] = 0;  // New slack variable column (second last column)
                }
            newTableau[numRows - 1, numCols - 2] = 1;  // New slack variable for the new constraint

            //Set the right - hand side(RHS) value for the new constraint

           double newRHSValue = Math.Floor(tableau[fracRow, numCols - 2]) - tableau[fracRow, numCols - 2];
           newTableau[numRows - 1, numCols - 1] = newRHSValue;

            //Update the tableau reference

           tableau = newTableau;

            //Ensure the basic variables are set correctly
            Simplex.InitializeTableau(tableau);
        }

        //Constraints is reg maar op verkeerde ry, rhs values verkeerd.
        private void AddCuttingPlane2(double[,] tableau, int numRows, int numCols, int fracRow, double fracValue)
        {
            // Increase the size of the tableau to accommodate the new constraint
            int newNumRows = numRows + 1;
            int newNumCols = numCols + 1;

            double[,] newTableau = new double[newNumRows, newNumCols];

            // Copy the old tableau into the new one, ensuring the RHS is updated correctly
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    newTableau[i, j] = tableau[i, j];
                }
            }

            // Add the new constraint (cutting plane)
            for (int j = 0; j < numCols - 1; j++)
            {
                newTableau[newNumRows - 2, j] = Math.Floor(tableau[fracRow, j]) - tableau[fracRow, j];
            }

            // Set the new slack variable column (second last column)
            for (int i = 0; i < newNumRows - 1; i++)
            {
                newTableau[i, newNumCols - 2] = 0;
            }
            newTableau[newNumRows - 2, newNumCols - 2] = 1;  // New slack variable for the new constraint

            // Set the right-hand side (RHS) value for the new constraint
            double newRHSValue = Math.Floor(tableau[fracRow, numCols - 1]) - tableau[fracRow, numCols - 1];
            newTableau[newNumRows - 2, newNumCols - 1] = newRHSValue;

            // Copy the objective function row into the new tableau
            for (int j = 0; j < numCols; j++)
            {
                newTableau[newNumRows - 1, j] = tableau[numRows - 1, j];
            }

            // Update the tableau reference in the Simplex object
            Simplex.InitializeTableau(newTableau);

            // Update the CuttingPlane's reference to the new tableau for future operations
            tableau = newTableau;
        }


    }
}

/*

Next Pivot Table:
Expected Rows: 3, Actual Rows: 3
Expected Cols: 5, Actual Cols: 5
    0,00     1,00     2,25 - 0,25     2,25
    1,00     0,00 - 1,25     0,25     3,75
    0,00     0,00     1,25     0,75    41,25

Optimal Solution Found:
x2 = 2,25
x4 = 3,75
Optimal value: 41,25
__________________________________________________________________________________________________
Applying Cutting Plane Method...
Updated Tableau with Cutting Plane:
Expected Rows: 4, Actual Rows: 4
Expected Cols: 6, Actual Cols: 6
    0,00     1,00     2,25 - 0,25     0,00     2,25
    1,00     0,00 - 1,25     0,25     0,00     3,75
    0,00     0,00 - 0,25 - 0,75 - 0,25     1,00
    0,00     0,00     1,25     0,75     1,25     0,00

Optimal Solution Found:
Error: Mismatch between basic variables and tableau rows.
basicVariables.Length: 2, Expected: 3
Next Pivot Table:
Expected Rows: 4, Actual Rows: 4
Expected Cols: 6, Actual Cols: 6
    0,00     1,00     2,25 - 0,25     0,00     2,25
    1,00     0,00 - 1,25     0,25     0,00     3,75
    0,00     0,00 - 0,25 - 0,75 - 0,25     1,00
    0,00     0,00     1,25     0,75     1,25     0,00

Optimal Solution Found:
x3 = 2,25
x4 = 3,75
x5 = 1,00
Optimal value: 0,00

*/

/*
 
private double[,] AddCuttingPlane(double[,] tableau, int numRows, int numCols, int fracRow, double fracValue, int[] basicVariables)
        {
            // Increase the size of the tableau to accommodate the new constraint
            double[,] updatedTableau = new double[numRows + 1, numCols + 1];

            // Copy the old tableau into the new one, skipping the second last row and column for now
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    if (i < numRows - 1 && j < numCols - 1) // Copy elements before the new row/column
                    {
                        updatedTableau[i, j] = tableau[i, j];
                    }
                    else if (i == numRows - 1) // Copy the last row to the new last row
                    {
                        updatedTableau[i + 1, j] = tableau[i, j];
                    }
                    else if (j == numCols - 1) // Copy the last column to the new last column
                    {
                        updatedTableau[i, j + 1] = tableau[i, j];
                    }
                }
            }

            // Add the new constraint (cutting plane) in the second last row
            for (int j = 0; j < numCols - 1; j++)
            {
                double fractionalPart = tableau[fracRow, j] - Math.Floor(tableau[fracRow, j]);
                updatedTableau[numRows - 1, j] = -fractionalPart; // Add negative fractional part to the new second last row
            }

            
            updatedTableau[numRows - 1, numCols] = updatedTableau[numRows - 1, numCols - 1];
            updatedTableau[numRows - 1, numCols - 1] = 1;

            // Set the third column value of the last two rows to be in the last column as well
            updatedTableau[numRows, numCols] = updatedTableau[numRows, numCols - 1];
            updatedTableau[numRows, numCols - 1] = 0;  

            // Update the basicVariables array to include the new basic variable
            int[] newBasicVariables = new int[basicVariables.Length + 1];

            // Copy the existing basic variables
            for (int i = 0; i < basicVariables.Length; i++)
            {
                newBasicVariables[i] = basicVariables[i];
            }

            // Add the new basic variable index (the new column index)
            newBasicVariables[newBasicVariables.Length - 1] = numCols - 1; // The new basic variable's column index

            // Update the basicVariables reference
            basicVariables = newBasicVariables;

            return updatedTableau; // Return the updated tableau
        }


 * */