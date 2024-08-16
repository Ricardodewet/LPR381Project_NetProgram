using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LP2
{
    internal class InputOutput
    {

        public void DisplaySolution(double[,] tableau, int numRows, int numCols, int[] basicVariables)
        {
            PrintTableau(tableau, numRows, numCols);

            Console.WriteLine("Optimal Solution Found:");

            if (basicVariables.Length != numRows - 1)
            {
                Console.WriteLine($"Error: Mismatch between basic variables and tableau rows.");
                Console.WriteLine($"basicVariables.Length: {basicVariables.Length}, Expected: {numRows - 1}");
                return; // or throw an exception
            }

            for (int i = 0; i < basicVariables.Length; i++)
            {
                Console.WriteLine($"x{basicVariables[i] + 1} = {tableau[i, numCols - 1]:F2}");
            }

            Console.WriteLine($"Optimal value: {tableau[numRows - 1, numCols - 1]:F2}");
            Console.WriteLine("__________________________________________________________________________________________________");
        }




        private void PrintTableau(double[,] tableau, int numRows, int numCols)
        {
            // Debugging: Print actual array dimensions
            int actualRows = tableau.GetLength(0);
            int actualCols = tableau.GetLength(1);

            Console.WriteLine($"Expected Rows: {numRows}, Actual Rows: {actualRows}");
            Console.WriteLine($"Expected Cols: {numCols}, Actual Cols: {actualCols}");

            if (numRows > actualRows || numCols > actualCols)
            {
                Console.WriteLine("Error: numRows or numCols exceeds the actual dimensions of the tableau array.");
                return;
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Console.Write($"{tableau[i, j],8:F2} ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

    }
}