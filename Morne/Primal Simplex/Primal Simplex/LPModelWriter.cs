using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Primal_Simplex
{
    public class LPModelWriter
    {
        public void WriteSolutionToFile(LPModel model, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                // Write the converted LP model in standard form
                writer.WriteLine("Converted LP Model:");
                writer.WriteLine("Objective Function:");
                writer.Write("Maximize "); // Assuming maximization; adjust for minimization
                for (int i = 0; i < model.Variables.Count; i++)
                {
                    writer.Write($"{model.Variables[i].Coefficient:F2}{model.Variables[i].Name} + ");
                }
                writer.WriteLine(); // Remove the last "+"

                writer.WriteLine("Subject to:");
                foreach (var constraint in model.Constraints)
                {
                    for (int i = 0; i < constraint.Variables.Count; i++)
                    {
                        writer.Write($"{constraint.Variables[i].Coefficient:F2}{constraint.Variables[i].Name} ");
                    }
                    writer.Write($"{constraint.Relation} {constraint.RightHandSide}");
                    writer.WriteLine();
                }

                // Write initial tableau
                WriteTableau(writer, model.Tableau, "Initial Tableau");

                // Write subsequent iterations
                for (int i = 1; i < model.Iterations.Count; i++)
                {
                    writer.WriteLine($"Tableau {i + 1}");
                    WriteTableau(writer, model.Iterations[i], $"Tableau {i + 1}");
                }

                // Write solution variables and objective value
                writer.WriteLine();
                foreach (var variable in model.Variables)
                {
                    double value = GetVariableValue(model, variable.Name);
                    writer.WriteLine($"{variable.Name} {value:F3}");
                }
                writer.WriteLine($"Objective Value: {model.ObjectiveValue:F3}");
            }
        }

        private void WriteTableau(StreamWriter writer, double[,] tableau, string title)
        {
            writer.WriteLine(title);
            writer.WriteLine();

            // Write column headers
            for (int j = 0; j < tableau.GetLength(1); j++)
            {
                if (j == tableau.GetLength(1) - 1)
                {
                    writer.Write("RHS"); // Right-hand side
                }
                else
                {
                    writer.Write($"x{j} ");
                }
            }
            writer.WriteLine();

            // Write each row of the tableau
            for (int i = 0; i < tableau.GetLength(0); i++)
            {
                for (int j = 0; j < tableau.GetLength(1); j++)
                {
                    writer.Write($"{tableau[i, j]:F3} ");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        private double GetVariableValue(LPModel model, string variableName)
        {
            //Find index of the variable in the model
            int variableIndex = model.Variables.FindIndex(v => v.Name == variableName);

            if (variableIndex == -1)
            {
                throw new ArgumentException($"Variable '{variableName}' not found.");
            }

            //Determine if the variable is basic or non-basic
            bool isBasic = model.Basis.Contains(variableIndex);

            if (isBasic)
            {
                int basisIndex = model.Basis.IndexOf(variableIndex);

                return model.Tableau[basisIndex, model.Tableau.GetLength(1) - 1];
            }
            else
            {
                return 0.0;
            }
        }
    }
}
