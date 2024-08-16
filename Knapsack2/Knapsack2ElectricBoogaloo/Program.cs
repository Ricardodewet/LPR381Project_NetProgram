using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knapsack;
using Knapsack2ElectricBoogaloo;

namespace Knapsack2ElectricBoogaloo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Knapsack2ElectricBoogaloo.Input_Class inputFile = new Knapsack2ElectricBoogaloo.Input_Class();
            Knapsack.List_Creation_Class listClass = new Knapsack.List_Creation_Class();

            List<List<string>> gottenList = listClass.GetList(inputFile.CheckInputFile());

            listClass.KnapsackStart(gottenList);
            listClass.DetermineRatio();
            listClass.Branch(listClass.constraints, listClass.branchedCheck);

            for (int x = listClass.branchAndBoundList.Count-1; x >= 0 ; x--)
            {
                for (int i = 0; i < listClass.branchAndBoundList[x].Count; i++)
                {
                    Console.WriteLine(listClass.branchAndBoundList[x][i] + " " + listClass.branchAndBoundCheckList[x][i]);
                }
                Console.WriteLine();
            }

            Console.ReadKey();
        }
    }
}
