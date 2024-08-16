using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapsack
{
    public class InputFile
    {
        public string CheckInputFile()
        {
            string filepath = @"C:\Users\berno\source\repos\Knapsack\Knapsack\Input File\input.txt";
            if (File.Exists(filepath))
            {
                var tfInput = File.ReadAllText(filepath);

                Console.Write(tfInput);

                return(tfInput);

            }
            else
            {
                Console.WriteLine("No Input Text File Found!");
                return(null);
            };
        }
    }
}
