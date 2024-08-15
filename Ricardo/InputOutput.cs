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
            for (int i = 0; i < numRows - 1; i++)
            {
                Console.WriteLine($"x{basicVariables[i] + 1} = {tableau[i, numCols - 1]}");
            }
            Console.WriteLine($"Optimal value: {tableau[numRows - 1, numCols - 1]}");

        }

        private void PrintTableau(double[,] tableau, int numRows, int numCols)
        {
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
