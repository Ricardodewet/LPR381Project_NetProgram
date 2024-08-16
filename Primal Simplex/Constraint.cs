using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex
{
    public class Constraint
    {
        public Constraint()
        {
            Variables = new List<Variable>();
        }

        public Constraint(List<Variable> variables, string relation, double rightHandSide)
        {
            Variables = variables;
            Relation = relation;
            RightHandSide = rightHandSide;
        }

        public string ConstraintNr { get; set; }
        public List<Variable> Variables { get; set; }
        public string Relation { get; set; } // <=, >=, =
        public double RightHandSide { get; set; }
    }
}
