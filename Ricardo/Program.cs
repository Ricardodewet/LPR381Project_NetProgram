using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LP2
{
    internal class Program
    {
        public enum Objective { Maximize, Minimize }
        public enum VariableType { Integer, Binary }
        public Objective ProblemObjective { get; set; }
        public VariableType VarType { get; set; }

        static void Main(string[] args)
        {
            try
            {
                // Select the objective function type
                Console.WriteLine("Select Objective (1 for Maximize, 2 for Minimize):");
                int objInput = int.Parse(Console.ReadLine());
                Objective objective = objInput == 1 ? Objective.Maximize : Objective.Minimize;

                // Select the variable type
                Console.WriteLine("Select Variable Type (1 for Integer, 2 for Binary):");
                int varTypeInput = int.Parse(Console.ReadLine());
                VariableType varType = varTypeInput == 1 ? VariableType.Integer : VariableType.Binary;

                // Enter the number of variables
                Console.WriteLine("Enter the number of variables:");
                int numVariables = int.Parse(Console.ReadLine());

                // Enter the number of constraints
                Console.WriteLine("Enter the number of constraints:");
                int numConstraints = int.Parse(Console.ReadLine());

                // Initialize the tableau dimensions
                int totalColumns = numVariables + numConstraints;
                double[,] tableau = new double[numConstraints + 1, totalColumns + 1];

                // Read Objective Function Coefficients
                Console.WriteLine("Enter the coefficients for the objective function (space-separated):");
                string[] objCoeffs = Console.ReadLine().Split();
                if (objCoeffs.Length != numVariables)
                {
                    throw new InvalidOperationException("Error: The number of coefficients does not match the number of variables.");
                }
                for (int i = 0; i < numVariables; i++)
                {
                    tableau[numConstraints, i] = objective == Objective.Maximize ? -double.Parse(objCoeffs[i]) : double.Parse(objCoeffs[i]);
                }
                tableau[numConstraints, totalColumns] = 0; // RHS of the objective function

                // Read Constraints and Handle Slack/Surplus Variables
                for (int i = 0; i < numConstraints; i++)
                {
                    Console.WriteLine($"Enter the coefficients for constraint {i + 1} (space-separated):");
                    string[] constraintCoeffs = Console.ReadLine().Split();
                    if (constraintCoeffs.Length != numVariables)
                    {
                        throw new InvalidOperationException($"Error: The number of coefficients for constraint {i + 1} does not match the number of variables.");
                    }
                    for (int j = 0; j < numVariables; j++)
                    {
                        tableau[i, j] = double.Parse(constraintCoeffs[j]);
                    }

                    Console.WriteLine("Enter the right-hand side (RHS) value for this constraint:");
                    tableau[i, totalColumns] = double.Parse(Console.ReadLine());

                    Console.WriteLine("Enter the sign restriction (<= for less than or equal to, >= for greater than or equal to):");
                    string signRestriction = Console.ReadLine();

                    // Add slack variable for <= constraints or surplus variable for >= constraints
                    if (signRestriction == "<=")
                    {
                        tableau[i, numVariables + i] = 1; // Add slack variable
                    }
                    else if (signRestriction == ">=")
                    {
                        // Convert constraint to <= by multiplying by -1
                        for (int j = 0; j < numVariables; j++)
                        {
                            tableau[i, j] = -tableau[i, j];
                        }
                        tableau[i, totalColumns] = -tableau[i, totalColumns];
                        tableau[i, numVariables + i] = -1; // Add surplus variable (as -1)
                    }
                    else
                    {
                        throw new InvalidOperationException("Error: Invalid sign restriction entered. Use '<=' or '>='.");
                    }
                }

                // Initialize and solve the problem
                InputOutput inout = new InputOutput();
                PrimalSimplex solver = new PrimalSimplex(objective, varType, null, inout);
                CuttingPlane cp = new CuttingPlane(solver);
                solver.cp = cp;

                solver.InitializeTableau(tableau);
                solver.Solve();
            }
            catch (FormatException ex)
            {
                // Handle format errors (e.g., parsing issues)
                Console.WriteLine("Input format error: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Handle specific operational errors (e.g., mismatched coefficients or invalid restrictions)
                Console.WriteLine("Operation error: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}





