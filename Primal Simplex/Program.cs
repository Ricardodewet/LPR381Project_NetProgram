namespace Primal_Simplex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Specify the input file path
            string inputFilePath = "C:\\Users\\morne\\OneDrive - belgiumcampus.ac.za\\3rd Year\\LPR381\\Project\\Primal Simplex Alogrithm\\Primal Simplex\\input.txt"; // Replace with your desired input file path

            //Read the LP model from the file
            LPModelReader reader = new LPModelReader();
            LPModel model = reader.ReadFromFile(inputFilePath);

            //Convert the model to canonical form
            LPModelConverter converter = new LPModelConverter();
            converter.ConvertToCanonicalForm(model);

            //Choose the solver
            bool useRevisedSimplex = false; //True- revised simplex, False - primal simplex

            //Solve the LPModel
            if (useRevisedSimplex)
            {
                model.SolveRevisedPrimalSimplex();
                //Write the solution to a file
                LPModelWriter writer = new LPModelWriter();
                writer.WriteSolutionToFile(model, "C:\\Users\\morne\\OneDrive - belgiumcampus.ac.za\\3rd Year\\LPR381\\Project\\Primal Simplex Alogrithm\\Primal Simplex\\output.txt");
            }
            else
            {
                model.SolvePrimalSimplex();
                // Write the solution to a file
                LPModelWriter writer = new LPModelWriter();
                writer.WriteSolutionToFile(model, "C:\\Users\\morne\\OneDrive - belgiumcampus.ac.za\\3rd Year\\LPR381\\Project\\Primal Simplex Alogrithm\\Primal Simplex\\output.txt");

            }


        }
    }
}
