using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex
{
    internal class PrimalSimplexSolver
    {
        protected readonly LPModel _model;

        public PrimalSimplexSolver(LPModel model)
        {
            _model = model;
            // Initialize the tableau here based on model dimensions
            _model.Tableau = CreateInitialTableau();
            _model.Basis = InitializeBasis();
        }

        public virtual void Solve()
        {
            while (!IsOptimal(_model.Tableau))
            {
                int pivotColumn = FindPivotColumn(_model.Tableau);
                if (pivotColumn == -1)
                {
                    // Handle unbounded solution case (optional)
                    return;
                }

                int pivotRow = FindPivotRow(_model.Tableau, pivotColumn);
                if (pivotRow == -1)
                {
                    // Handle infeasible solution case (optional)
                    return;
                }

                Pivot(_model.Tableau, pivotRow, pivotColumn);

                // Store the current tableau for text file
                _model.Iterations.Add(CopyTableau(_model.Tableau));

                // Update objective value
                UpdateObjectiveValue();
            }

            ExtractSolution(_model.Tableau);
        }

        private double[,] CopyTableau(double[,] tableau)
        {
            int rows = tableau.GetLength(0);
            int cols = tableau.GetLength(1);
            double[,] copy = new double[rows, cols];
            Array.Copy(tableau, copy, rows * cols);
            return copy;
        }

        //Solve primalSimplex methods
        private double[,] CreateInitialTableau()
        {
            int numRows = _model.Constraints.Count;
            int numCols = _model.Variables.Count + numRows + 1; // +1 for RHS

            double[,] tableau = new double[numRows + 1, numCols];

            //Fill in constraint coefficients
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < _model.Variables.Count; j++)
                {
                    tableau[i, j] = _model.Constraints[i].Variables[j].Coefficient;
                }//end for

                //Fill in identity matrix for slack, surplus, and artificial variables
                int colIndex = _model.Variables.Count;
                foreach (var var in _model.Constraints[i].Variables.Skip(_model.Variables.Count))
                {
                    tableau[i, colIndex] = var.Name.StartsWith("s") || var.Name.StartsWith("a") ? 1 : -1;
                    colIndex++;
                }//end foreach
                tableau[i, numCols - 1] = _model.Constraints[i].RightHandSide;
            }//end for

            //Fill in objective function coefficients
            for (int j = 0; j < _model.Variables.Count; j++)
            {
                tableau[numRows, j] = -_model.Variables[j].Coefficient * (_model.IsMaximisation ? 1 : -1); //Multiply by -1 for minimisation
            }
            return tableau;
        }

        private List<int> InitializeBasis()
        {
            List<int> basis = new List<int>();
            int slackIndex = _model.Variables.Count;
            int artificialIndex = slackIndex + _model.Constraints.Count;

            for (int i = 0; i < _model.Constraints.Count; i++)
            {
                if (_model.Constraints[i].Relation == "<=")
                {
                    basis.Add(slackIndex + i); // Add slack variable index to basis
                }
                else if (_model.Constraints[i].Relation == ">=")
                {
                    basis.Add(artificialIndex + i); // Add artificial variable index to basis
                }
                else // Equality constraint
                {
                    basis.Add(artificialIndex + i); // Add artificial variable index to basis
                }
            }
            return basis;
        }

    private bool IsOptimal(double[,] tableau)
        {
            int numCols = tableau.GetLength(1);
            int objRow = tableau.GetLength(0) - 1;

            //Check if all coefficients in the objective row are non-negative (max) or non-positive (min)
            for (int j = 0; j < numCols - 1; j++)
            {
                if (_model.IsMaximisation && tableau[objRow, j] < 0 || !_model.IsMaximisation && tableau[objRow, j] > 0)
                {
                    return false;
                }//end if
            }//end for
            return true;
        }

        private int FindPivotColumn(double[,] tableau)
        {
            int numCols = tableau.GetLength(1);
            int objRow = tableau.GetLength(0) - 1;

            int pivotColumn = -1;
            double minCoeff = _model.IsMaximisation ? double.MaxValue : double.MinValue;

            for (int j = 0; j < numCols - 1; j++)//- 1 excludes RHS col
            {
                if (_model.IsMaximisation && tableau[objRow, j] < minCoeff || !_model.IsMaximisation && tableau[objRow, j] > minCoeff)
                {
                    minCoeff = tableau[objRow, j];
                    pivotColumn = j;
                }//end if
            }//end for
            return pivotColumn;
        }

        private int FindPivotRow(double[,] tableau, int pivotColumn)
        {
            int numRows = tableau.GetLength(0) - 1; // Exclude objective row
            int numCols = tableau.GetLength(1);
            int pivotRow = -1;
            double minRatio = double.MaxValue;

            for (int i = 0; i < numRows; i++)
            {
                if (tableau[i, pivotColumn] > 0) // Check for positive coefficient
                {
                    double ratio = tableau[i, numCols - 1] / tableau[i, pivotColumn];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            // Check for infeasibility (all coefficients in pivot column are zero)
            if (pivotRow == -1 && Math.Abs(tableau[0, pivotColumn]) < 1e-6) // Check first row for efficiency
            {
                return -1;
            }

            return pivotRow;
        }

        private void Pivot(double[,] tableau, int pivotRow, int pivotColumn)
        {
            int numRows = tableau.GetLength(0);
            int numCols = tableau.GetLength(1);

            //Divide pivot row by pivot element
            for (int j = 0; j < numCols; j++)
            {
                tableau[pivotRow, j] /= tableau[pivotRow, pivotColumn];
            }

            //Eliminate other elements in pivot column
            for (int i = 0; i < numRows; i++)
            {
                if (i != pivotRow)
                {
                    double factor = tableau[i, pivotColumn];
                    for (int j = 0; j < numCols; j++)
                    {
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                    }
                }
            }
        }

        private void ExtractSolution(double[,] tableau)
        {
            int numRows = tableau.GetLength(0);
            int numCols = tableau.GetLength(1);

            //Identify basic variables 
            List<int> basicVariables = new List<int>();
            for (int i = 0; i < numRows - 1; i++)
            {
                for (int j = 0; j < numCols - 1; j++)
                {
                    if (Math.Abs(tableau[i, j] - 1) < 0.000001)
                    {
                        basicVariables.Add(j);
                        break;
                    }
                }
            }

            //Extract solution
            for (int i = 0; i < basicVariables.Count; i++)
            {
                Console.WriteLine($"x{basicVariables[i]} = {tableau[i, numCols - 1]:F3}");
            }
        }

        private void UpdateObjectiveValue()
        {
            _model.ObjectiveValue = 0;
            for (int i = 0; i < _model.Variables.Count; i++)
            {
                int basisIndex = _model.Basis.IndexOf(i);
                if (basisIndex != -1)
                {
                    _model.ObjectiveValue += _model.Variables[i].Coefficient * _model.Tableau[basisIndex, _model.Tableau.GetLength(1) - 1];
                }
            }
        }     
    }

}
