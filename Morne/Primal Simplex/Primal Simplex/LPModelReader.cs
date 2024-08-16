using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Primal_Simplex
{
    public enum SignRestriction
    {
        NonNegative,
        NonPositive,
        Free,
        Integer,
        Binary
    }

    public enum Relation
    {
        LessThanOrEqual,
        GreaterThanOrEqual,
        Equal
    }

    public class LPModelReader
    {
        private static readonly Dictionary<string, SignRestriction> SignRestrictionMap = new()
        {
            { "+", SignRestriction.NonNegative },
            { "-", SignRestriction.NonPositive },
            { "urs", SignRestriction.Free },
            { "int", SignRestriction.Integer },
            { "bin", SignRestriction.Binary }
        };

        private static readonly Dictionary<string, Relation> RelationMap = new()
        {
            { "<=", Relation.LessThanOrEqual },
            { ">=", Relation.GreaterThanOrEqual },
            { "=", Relation.Equal }
        };

        public LPModel ReadFromFile(string filename)
        {
            var lines = File.ReadAllLines(filename);
            LPModel model = new LPModel();

            // Parse objective function
            ParseObjectiveFunction(lines[0], model);

            // Parse constraints
            for (int i = 1; i < lines.Length - 1; i++)
            {
                ParseConstraint(lines[i], model);
            }

            // Parse sign restrictions (if present)
            if (lines.Length > 1)
            {
                ParseSignRestrictions(lines[lines.Length - 1], model);
            }

            return model;
        }

        private void ParseObjectiveFunction(string line, LPModel model)
        {
            // Split the line into parts
            var parts = line.Trim().Split(' ');

            // Determine maximization or minimization
            model.IsMaximisation = parts[0].ToLower() == "max";

            // Create variables and coefficients
            for (int i = 1; i < parts.Length; i++)
            {
                var signAndCoeff = new String(parts[i].ToArray());
                bool isNegative = signAndCoeff[0] == '-';

                if (!double.TryParse(signAndCoeff[1].ToString(), out double coefficient))
                {
                    throw new FormatException($"Invalid coefficient value at line 1: {signAndCoeff[1]}");
                }

                if (isNegative)
                {
                    coefficient = coefficient * -1;
                }

                var variableName = i.ToString();
                
                //Add the variable "names" to variable (this is used to check if the constraints has the same amount of variables, as the objective function)
                Variable variable = new Variable { Name = variableName};

                //Add the objective function's coefficients to the LPModel
                model.ObjectiveFunctionCoefficients.Add(coefficient);
            }
        }

        private void ParseConstraint(string line, LPModel model)
        {
            //1. Number the constraint
            int constraintNr = model.Constraints.Count() +1;

            //Split the line to find the relevant details to populate the constraint class with
            var parts = line.Trim().Split(' ');

            //4. Extract relation (sign) 
            var relationString = parts[parts.Length - 2];

            if (!RelationMap.TryGetValue(relationString, out var relation))
            {
                throw new FormatException($"Invalid relation: {relationString}");
            }

            //3. Extract right-hand side
            if (!double.TryParse(parts[parts.Length - 1], out double rightHandSide))
            {
                throw new FormatException($"Invalid right-hand side value: {parts[parts.Length - 1]}");
            }

            //2. Parse variables and coefficients
            List<Variable> constraintVariables = new List<Variable>();

            for (int i = 0; i < parts.Length - 2; i++)
            {
                var signAndCoeff = new String(parts[i].ToArray());
                bool isNegative = signAndCoeff[0] == '-';

                if (!double.TryParse(signAndCoeff[1].ToString(), out double coefficient))
                {
                    throw new FormatException($"Invalid coefficient value at line 1: {signAndCoeff[1]}");
                }

                if (isNegative)
                {
                    coefficient = coefficient * -1;
                }

                var variableName = i.ToString();

                Variable variable = new Variable();
                variable.Name = variableName;
                variable.Coefficient = coefficient;

                constraintVariables.Add(variable);
            }

            //Create the new constraint
            Constraint constraint = new Constraint();
            constraint.ConstraintNr = constraintNr.ToString();
            constraint.Variables = constraintVariables;
            constraint.Relation = relation.ToString();
            constraint.RightHandSide = rightHandSide;

            model.Constraints.Add(constraint);
        }

        private void ParseSignRestrictions(string line, LPModel model)
        {
            var variables = line.Trim().Split(' ');

            if (variables.Length != model.ObjectiveFunctionCoefficients.Count)
            {
                throw new ArgumentException("Number of variables in sign restrictions does not match objective function");
            }

            for (int i = 0; i < variables.Length; i++)
            {
                var variable = model.ObjectiveFunctionCoefficients[i];
                var signRestrictionString = variables[i];

                if (SignRestrictionMap.TryGetValue(signRestrictionString, out var signRestriction))
                {
                    model.SignRestriction.Add(signRestriction.ToString());
                }
                else
                {
                    throw new ArgumentException($"Invalid sign restriction: {signRestrictionString}");
                }
            }
        }
    }
}
