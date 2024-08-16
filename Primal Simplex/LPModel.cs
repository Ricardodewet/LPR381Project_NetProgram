using Primal_Simplex.Primal_Simplex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex
{
    public class LPModel
    {
        public LPModel()
        {
            IsMaximisation = false;
            Variables = new List<Variable>();
            Constraints = new List<Constraint>();
            ObjectiveFunctionCoefficients = new List<double>();
            Basis = new List<int>();
            ObjectiveValue = 0;
            InverseBasis = null;
            SignRestriction = new List<string>();
        }

        public LPModel(bool isMaximisation, List<Variable> variables, List<Constraint> constraints, List<double> objectiveFunctionCoefficients)
        {
            IsMaximisation = isMaximisation;
            Variables = variables;
            Constraints = constraints;
            ObjectiveFunctionCoefficients = objectiveFunctionCoefficients;
        }

        public bool IsMaximisation { get; set; }
        public List<Variable> Variables { get; set; }
        public List<Constraint> Constraints { get; set; }
        public List<double> ObjectiveFunctionCoefficients { get; set; }
        public double[,] Tableau { get; set; }
        public List<int> Basis { get; set; }
        public double ObjectiveValue { get; set; }
        public double[,] InverseBasis { get; set; }
        public List<string> SignRestriction { get; set; } // +, -, urs, int, bin
        public List<double[,]> Iterations { get; set; } = new List<double[,]>(); //For storing table iterations


        public void SolvePrimalSimplex()
        {
            var solver = new PrimalSimplexSolver(this);
            solver.Solve();
        }

        public void SolveRevisedPrimalSimplex()
        {
            var solver = new RevisedPrimalSimplexSolver(this);
            solver.Solve();
        }
    }
}
