using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex
{
    public class Variable
    {
        public Variable()
        {
        }

        public Variable(string name, double coefficient, string signRestriction)
        {
            this.Name = name;
            this.Coefficient = coefficient;
            this.SignRestriction = signRestriction;
        }

        public string Name { get; set; }
        public double Coefficient { get; set; }
        public string SignRestriction { get; set; } // +, -, urs, int, bin
    }
}
