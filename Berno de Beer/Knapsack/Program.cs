using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knapsack;

namespace Knapsack
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Knapsack.InputFile inputFile = new Knapsack.InputFile();
            Knapsack.ListClass listClass = new Knapsack.ListClass();

            List<List<string>> gottenList = listClass.GetList(inputFile.CheckInputFile());

            listClass.KnapsackStart(gottenList);
            listClass.DetermineRatio();
            listClass.Branch(listClass.constraints, listClass.branchedCheck);
        }
    }
}
