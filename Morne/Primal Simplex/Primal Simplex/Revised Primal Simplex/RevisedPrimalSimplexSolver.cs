using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex.Primal_Simplex
{
    internal class RevisedPrimalSimplexSolver : PrimalSimplexSolver
    {
        public RevisedPrimalSimplexSolver(LPModel model) : base(model)
        {
            _model.InverseBasis = IdentityMatrix(_model.Constraints.Count);
            _model.Tableau = CreateRevisedInitialTableau();
        }

        public override void Solve()
        {
            SolveRevisedPrimalSimplex();
        }

        public void SolveRevisedPrimalSimplex()
        {
            // Initalise basis and inverse basis
            List<int> basis = _model.Basis.ToList();
            double[,] inverseBasis = IdentityMatrix(basis.Count);

            while (!IsOptimal())
            {
                // Calculate reduced costs
                double[] reducedCosts = CalculateReducedCosts();

                // Find entering variable
                int enteringVariable = FindEnteringVariable(reducedCosts);

                // Calculate entering column
                double[] enteringColumn = CalculateEnteringColumn(enteringVariable);

                // Find leaving variable
                int leavingVariable = FindLeavingVariable(enteringColumn);

                // Pivot
                Pivot(enteringVariable, leavingVariable);

                // Update objective value
                UpdateObjectiveValue();
            }

            // Extract solution
            ExtractSolution(_model.InverseBasis, _model.Tableau, basis);
        }

        private double[,] CreateRevisedInitialTableau()
        {
            int numConstraints = _model.Constraints.Count;
            int numVariables = _model.Variables.Count;

            // Create the tableau with appropriate dimensions
            double[,] tableau = new double[numConstraints, numConstraints + 1];

            // Fill in the identity matrix (basis)
            for (int i = 0; i < numConstraints; i++)
            {
                tableau[i, i] = 1;
            }

            // Fill in the right-hand side values
            for (int i = 0; i < numConstraints; i++)
            {
                tableau[i, numConstraints] = _model.Constraints[i].RightHandSide;
            }

            return tableau;
        }

        private bool IsOptimal()
        {
            double[] reducedCosts = CalculateReducedCosts();
            return reducedCosts.All(rc => (_model.IsMaximisation ? rc <= 0 : rc >= 0));
        }

        private double[] CalculateReducedCosts()
        {
            int numVariables = _model.Variables.Count;
            int numConstraints = _model.Constraints.Count;

            double[] reducedCosts = new double[numVariables];
            double[] dualPrices = new double[numConstraints];

            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numConstraints; j++)
                {
                    // Check for null values before accessing elements
                    if (_model.InverseBasis != null && _model.Tableau != null)
                    {
                        Console.WriteLine(i + ", " + j + ", " + _model.InverseBasis[i, j] + ", " + _model.Tableau[j, numConstraints - 1]);
                        dualPrices[i] += _model.InverseBasis[i, j] * _model.Tableau[j, numConstraints - 1];
                    }
                    else
                    {
                        throw new Exception("Tableau or InverseBasis is null");
                    }
                }
            }

            for (int j = 0; j < numVariables; j++)
            {
                double sum = 0;
                for (int i = 0; i < numConstraints; i++)
                {
                    sum += dualPrices[i] * _model.Tableau[i, j];
                }
                reducedCosts[j] = _model.Tableau[numConstraints, j] - sum;
            }

            return reducedCosts;
        }

        private int FindEnteringVariable(double[] reducedCosts)
        {
            int enteringVariable = -1;
            double minReducedCost = _model.IsMaximisation ? double.MaxValue : double.MinValue;

            for (int j = 0; j < reducedCosts.Length; j++)
            {
                if (_model.IsMaximisation && reducedCosts[j] < minReducedCost || !_model.IsMaximisation && reducedCosts[j] > minReducedCost)
                {
                    minReducedCost = reducedCosts[j];
                    enteringVariable = j;
                }
            }

            return enteringVariable;
        }

        private double[] CalculateEnteringColumn(int enteringVariable)
        {
            int numConstraints = _model.InverseBasis.GetLength(0);
            double[] enteringColumn = new double[numConstraints];

            for (int i = 0; i < numConstraints; i++)
            {
                for (int j = 0; j < numConstraints; j++)
                {
                    enteringColumn[i] += _model.InverseBasis[i, j] * _model.Tableau[j, enteringVariable];
                }
            }

            return enteringColumn;
        }

        private int FindLeavingVariable(double[] enteringColumn)
        {
            int numConstraints = enteringColumn.Length;
            int leavingVariableIndex = -1;
            double minRatio = double.MaxValue;

            for (int i = 0; i < numConstraints; i++)
            {
                if (enteringColumn[i] > 0)
                {
                    double ratio = _model.Tableau[i, _model.Tableau.GetLength(1) - 1] / enteringColumn[i];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        leavingVariableIndex = i;
                    }
                }
            }

            return _model.Basis[leavingVariableIndex];
        }

        private void Pivot(int enteringVariable, int leavingVariable)
        {
            // Update basis
            _model.Basis[_model.Basis.IndexOf(leavingVariable)] = enteringVariable;

            // Update inverse basis
            UpdateInverseBasis(enteringVariable, leavingVariable);
            _model.Iterations.Add(CopyTableau(_model.Tableau));
        }

        private double[,] CopyTableau(double[,] tableau)
        {
            int rows = tableau.GetLength(0);
            int cols = tableau.GetLength(1);
            double[,] copy = new double[rows, cols];
            Array.Copy(tableau, copy, rows * cols);
            return copy;
        }

        private void UpdateObjectiveValue()
        {
            _model.ObjectiveValue = 0;
            for (int i = 0; i < _model.Basis.Count; i++)
            {
                int basicVariableIndex = _model.Basis[i];
                _model.ObjectiveValue += _model.Variables[basicVariableIndex].Coefficient * _model.Tableau[i, _model.Tableau.GetLength(1) - 1];
            }
        }

        private void ExtractSolution(double[,] inverseBasis, double[,] tableau, List<int> basis)
        {
            int numConstraints = inverseBasis.GetLength(0);
            Console.WriteLine("_model.Variables.Count: " + _model.Variables.Count);

            double[] solution = new double[_model.Variables.Count];

            for (int i = 0; i < numConstraints; i++)
            {
                int basicVariableIndex = basis[i];
                double value = 0;
                for (int j = 0; j < numConstraints; j++)
                {
                    value += inverseBasis[i, j] * tableau[j, tableau.GetLength(1) - 1];
                }
                Console.WriteLine(basicVariableIndex);
                Console.WriteLine(value);
                Console.WriteLine($"Solution size: {solution.Length}");
                foreach (var item in solution)
                {
                    Console.WriteLine(item);
                }
                solution[basicVariableIndex] = value;
            }

            //Print the solution
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine($"x{i} = {solution[i]:F3}");
            }
        }

        private double[,] IdentityMatrix(int size)
        {
            double[,] identityMatrix = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    identityMatrix[i, j] = (i == j) ? 1 : 0;
                }
            }

            return identityMatrix;   

        }

        private void UpdateInverseBasis(int enteringVariable, int leavingVariable)
        {
            int numConstraints = _model.InverseBasis.GetLength(0);
            double[] eta = new double[numConstraints];

            for (int i = 0; i < numConstraints; i++)
            {
                eta[i] = _model.InverseBasis[i, leavingVariable];
            }

            for (int i = 0; i < numConstraints; i++)
            {
                _model.InverseBasis[i, leavingVariable] = -eta[i] / _model.Tableau[leavingVariable, enteringVariable];
                for (int j = 0; j < numConstraints; j++)
                {
                    if (j != leavingVariable)
                    {
                        _model.InverseBasis[i, j] += eta[j] * _model.InverseBasis[i, leavingVariable];
                    }
                }
            }            
        }
    }
}
