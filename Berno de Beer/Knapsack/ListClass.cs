using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knapsack;

namespace Knapsack
{
    public class ListClass
    {
        Knapsack.InputFile InputFile { get; set; }


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
                    string outputItem = "";
                    foreach (var item in subList)
                    {
                        outputItem += item;
                    }
                    Console.WriteLine(outputItem);
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
            length = lList[0].Count() - 1;


            foreach (var item in lList[0])
            {
                if (!lList[0][0].Contains(item))
                {
                    varX.Add(Double.Parse(item));
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


            foreach (var item in lList[2])
            {
                signRestrictions.Add(item);
            }
        }

        public void DetermineRatio()
        {
            List<double> ratio = new List<double>();
            int i = 0;

            foreach (var item in varX)
            {
                ratio.Add(item / constraints[i]);
                i++;
            }

            bool checkRatio = false;

            while (!checkRatio)
            {
                checkRatio = true;

                for (i = 0; i < ratio.Count-1; i++)
                {
                    if (ratio[i] < ratio[i + 1])
                    {
                        double buffer;
                        buffer = ratio[i];
                        ratio[i] = ratio[i + 1];
                        ratio[i + 1] = buffer;

                        buffer = varX[i];
                        varX[i] = varX[i + 1];
                        varX[i + 1] = buffer;

                        buffer = constraints[i];
                        constraints[i] = constraints[i + 1];
                        constraints[i + 1] = buffer;

                        string strBuffer = signRestrictions[i];
                        signRestrictions[i] = signRestrictions[i + 1];
                        signRestrictions[i + 1] = strBuffer;

                        checkRatio = false;
                    }
                }

            }

        }

        public List<List<double>> branchAndBoundList = new List<List<double>>();
        public List<List<int>> branchAndBoundCheckList = new List<List<int>>();

        public void Branch(List<double> branchList, List<int> checkList)
        {
            var testLimit = Double.Parse(limit);

            bool continueCheck = true;
            var i = 0;
            while (continueCheck)
            {
                continueCheck = false;
                
                    if (testLimit - branchList[i] > 0)
                    {
                        testLimit -= branchList[i];

                    if (i < branchList.Count - 1)
                    {
                        //foreach (var item in branchList)
                        //{
                        //    Console.Write(item + " ");
                        //}
                        //Console.WriteLine();
                        i++;
                    }
                    else
                    {
                        branchAndBoundList.Add(branchList);
                        branchAndBoundCheckList.Add(checkList);
                        break;
                    }

                    continueCheck = true;
                        //Console.WriteLine(testLimit);
                    }
                    else if (testLimit - branchList[i] == 0)
                    {
                        branchAndBoundList.Add(branchList);
                        branchAndBoundCheckList.Add(checkList);
                        Console.WriteLine("b");
                    }
                    else
                    {
                        branchAndBoundList.Add(branchList);
                        branchAndBoundCheckList.Add(checkList);
                        Console.WriteLine("c");
                        List<double> subOne = new List<double>();
                        List<double> subTwo = new List<double>();
                        List<int> subCheck = new List<int>();

                        subOne = branchList;
                        subTwo = branchList;
                        subCheck = checkList;

                        subOne[i] = 0;
                        subCheck[i] = 1;

                        bool subCheckSort = false;
                        bool notOptimal = false;
                        for (int x = 0; x < subCheck.Count; x++)
                        {
                            if (subCheck[x] == 0)
                            {
                                notOptimal = true;
                            }
                        }
                        if (notOptimal)
                        {
                            while (!subCheckSort)
                            {
                                subCheckSort = true;
                                for (int j = 0; j < checkList.Count - 1; j++)
                                {
                                    if (checkList[j] < checkList[j + 1])
                                    {
                                        subCheckSort = false;
                                        int buffer = checkList[j];
                                        checkList[j] = checkList[j + 1];
                                        checkList[j + 1] = buffer;

                                        double subProbBuffer;
                                        subProbBuffer = subOne[j];
                                        subOne[j] = subOne[j + 1];
                                        subOne[j + 1] = subProbBuffer;

                                        subProbBuffer = subTwo[j];
                                        subTwo[j] = subTwo[j + 1];
                                        subTwo[j + 1] = subProbBuffer;

                                    }
                                }
                            }
                        foreach (var item in subOne)
                        {
                            Console.WriteLine("One: "+ item);
                        }

                        Branch(subOne, subCheck);
                        
                        foreach (var item in subTwo)
                        {
                            Console.WriteLine("Two: " +item);
                        }
                        Branch(subTwo, subCheck);
                        
                        }
                    }
                


            }
        }
    }
}
