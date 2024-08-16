using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapsack
{
    public class List_Creation_Class
    {


        public List<List<string>> GetList(string inputTextFile)
        {
            var list = new List<List<string>>();
            inputTextFile += " \n";
            while (inputTextFile != "")
            {
                var newString = inputTextFile.Substring(0, inputTextFile.IndexOf("\n") - 1);
                //newString = newString.Remove(tfInput.IndexOf("\n"));
                newString += " ";

                var subList = new List<string>();
                if (newString != " ")
                {
                    while (newString != "")
                    {
                        subList.Add(newString.Substring(0, newString.IndexOf(" ")));
                        newString = newString.Remove(0, newString.IndexOf(" ") + 1);
                    }

                    list.Add(subList);
                }

                inputTextFile = inputTextFile.Remove(0, inputTextFile.IndexOf("\n") + 1);
            }
            return list;
        }

        public void DisplayList(List<List<string>> lList)
        {
            if (lList.Count == 3)
            {
                foreach (var subList in lList)
                {
                    Console.WriteLine(string.Join("", subList));
                }
            }
            else
            {
                Console.WriteLine("Incorrect amount of constraints.");
            }
        }

        List<double> varX = new List<double>();
        public List<double> constraints = new List<double>();
        List<string> signRestrictions = new List<string>();
        public List<int> branchedCheck = new List<int>();
        string minmax;
        string limit;
        int length;

        public void KnapsackStart(List<List<string>> lList)
        {
            minmax = lList[0][0];
            limit = lList[1][lList[1].Count() - 1];
            limit = limit.Remove(0, limit.IndexOf("=") + 1);
            length = lList[0].Count - 1;

            foreach (var item in lList[0].Skip(1))
            {
                if (double.TryParse(item, out double value))
                {
                    varX.Add(value);
                }
            }

            foreach (var item in lList[1])
            {
                if (!lList[1][lList[1].Count() - 1].Contains(item))
                {
                    constraints.Add(Double.Parse(item));
                    branchedCheck.Add(0);
                }
            }

            signRestrictions.AddRange(lList[2]);
        }

        public void DetermineRatio()
        {
            var ratios = varX.Zip(constraints, (vx, c) => vx / c).ToList();
            var indices = Enumerable.Range(0, ratios.Count).ToList();

            indices.Sort((i1, i2) => ratios[i2].CompareTo(ratios[i1]));

            varX = indices.Select(i => varX[i]).ToList();
            constraints = indices.Select(i => constraints[i]).ToList();
            signRestrictions = indices.Select(i => signRestrictions[i]).ToList();
        }

        public List<List<double>> branchAndBoundList = new List<List<double>>();
        public List<List<int>> branchAndBoundCheckList = new List<List<int>>();

        void ProcessBranch(List<double> currentList, List<int> currentCheckList, int startIndex)
        {
            var testLimit = Double.Parse(limit);
            for (int i = 0; i < currentList.Count; i++)
            {
                if (testLimit - currentList[i] > 0)
                {
                    testLimit -= currentList[i];

                    if (i == currentList.Count - 1)
                    {
                        branchAndBoundList.Add(new List<double>(currentList));
                        branchAndBoundCheckList.Add(new List<int>(currentCheckList));
                        
                        break;
                    }

                    
                }
                else
                {
                    var subList = new List<double>(currentList) { [i] = 0 };
                    var subCheckList = new List<int>(currentCheckList) { [i] = 1 };

                    if (subCheckList.Any(v => v == 0))
                    {
                        ProcessBranch(subList, subCheckList, i + 1);
                    }
                }
            }
        }

        public void Branch(List<double> branchList, List<int> checkList)
        {
            ProcessBranch(branchList, checkList, 0);
        }
    }
}
