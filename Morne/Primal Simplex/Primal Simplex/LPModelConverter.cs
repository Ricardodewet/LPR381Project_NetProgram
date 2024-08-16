using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primal_Simplex
{
    public class LPModelConverter
    {
        private const double M = 10000;

        public void ConvertToCanonicalForm(LPModel model)
        {
            //Introduce slack variables for <= constraints
            foreach (var constraint in model.Constraints.Where(c => c.Relation == "<="))
            {
                var slackVar = new Variable($"s{model.Constraints.IndexOf(constraint)}", 1 , "+");
                constraint.Variables.Add(slackVar);
                model.Variables.Add(slackVar);
                constraint.Relation = "=";
            }

            //Introduce surplus and artificial variables for >= constraints
            foreach (var constraint in model.Constraints.Where(c => c.Relation == ">="))
            {
                var surplusVar = new Variable ($"e{model.Constraints.IndexOf(constraint)}", 1, "+");
                var artificialVar = new Variable ($"a{model.Constraints.IndexOf(constraint)}", 1, "+");
                constraint.Variables.Add(surplusVar);
                constraint.Variables.Add(artificialVar);
                model.Variables.Add(surplusVar);
                model.Variables.Add(artificialVar);
                constraint.Relation = "=";

                //Add artificial variable to objective function 
                model.ObjectiveFunctionCoefficients.Add(-M); // Assuming M is a large positive constant
            }

            //Add artificial variables to objective function with large negative coefficients 
            //for two-phase simplex
            foreach (var constraint in model.Constraints.Where(c => c.Relation == "="))
            {
                var artificialVar = new Variable ($"a{model.Constraints.IndexOf(constraint)}", 1, "+");
                constraint.Variables.Add(artificialVar);
                model.Variables.Add(artificialVar);
                model.ObjectiveFunctionCoefficients.Add(-M); //M is a large pos number
            }
        }
    }
}
