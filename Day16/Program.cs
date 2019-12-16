using System;
using System.Collections.Generic;
using System.Linq;

namespace Day16
{
    class Program
    {
        static void Main(string[] args)
        {
            Part2();
            
        }

        public static void Part1()
        {
            var inputList = PuzzleInputParser.FileReader.GetValues("./input.txt");
            var basePattern = new List<int> { 0, 1, 0, -1 };

            for (int i = 0; i < 100; i++)
            {
                var result = FlawedFrequencyTransmission(inputList, basePattern);
                inputList = result;

                Console.WriteLine($"After {i + 1} pase: {string.Join("", result)}");
            }
        }

        public static void Part2()
        {
            var inputList = PuzzleInputParser.FileReader.GetValues("./input.txt");
            var messageOffset = int.Parse(string.Join("", inputList.Take(7)));
            Console.WriteLine($"Message offset: {string.Join("", messageOffset)}");
            int numberRepeatCount = 10000;
            var inWhatListFloat = messageOffset / (float)numberRepeatCount;
            var inWhatList = (int)Math.Floor(inWhatListFloat);

            var inputLists = new List<List<int>>();
            for (int i = 0; i < numberRepeatCount; i++)
            {
                inputLists.Add(inputList);
            }

            var basePattern = new List<int> { 0, 1, 0, -1 };

            for (int i = 0; i < 100; i++)
            {
                FlawedFrequencyTransmissionV2(inputLists, basePattern, inWhatList);
                Console.WriteLine($"After {i + 1} phase");
            }
            
            var positionInList = (int)Math.Floor(inputList.Count() * (inWhatListFloat - (float)inWhatList));

            var result = inputLists[inWhatList].Skip(positionInList - 1).Take(8);

            Console.WriteLine($"Magic result {string.Join("", result)}");
        }

        public static List<int> FlawedFrequencyTransmission(List<int> input, List<int> basePattern)
        {
            var returnList = new List<int>();

            for (int i = 0; i < input.Count; i++)
            {
                var tempList = new List<int>();
                var pattern = CalculateActualPatternToUse(basePattern, input.Count, i + 1);

                for (int p = 0; p < input.Count; p++)
                {
                    tempList.Add(input[p] * pattern[p]);
                }

                returnList.Add(
                    int.Parse(tempList.Sum().ToString().TakeLast(1).First().ToString())
                );
            }
            
            return returnList;
        }

        public static void FlawedFrequencyTransmissionV2(List<List<int>> inputs, List<int> basePattern, int inWhatList)
        {
            var inputIndex = 0;
            foreach(var input in inputs)
            {
                if (inputIndex < inWhatList) {
                    inputIndex++;
                    continue; 
                }
                else if(inputIndex > inWhatList)
                {
                    break;
                }

                for (int i = 0; i < input.Count; i++)
                {
                    var tempList = new List<int>();
                    var pattern = CalculateActualPatternToUseV2(basePattern, input.Count, i + 1, inputIndex);

                    for (int p = 0; p < input.Count; p++)
                    {
                        tempList.Add(input[p] * pattern[p]);
                    }

                    inputs[inputIndex][i] = int.Parse(tempList.Sum().ToString().TakeLast(1).First().ToString());
                }

                inputIndex++;
            }
        }

        public static List<int> CalculateActualPatternToUse(List<int> basePattern, int lengthOfList, int numbersToUse)
        {
            var returnList = new List<int>();

            while(returnList.Count <= lengthOfList)
            {
                foreach(var item in basePattern)
                {
                    for (int i = 0; i < numbersToUse; i++)
                    {
                        returnList.Add(item);
                    }
                }
            }

            returnList.RemoveAt(0);

            if(returnList.Count > lengthOfList)
            {
                var numbersToRemove = returnList.Count - lengthOfList;
                returnList.RemoveRange(returnList.Count - numbersToRemove, numbersToRemove);
            }

            return returnList;
        }

        public static List<int> CalculateActualPatternToUseV2(List<int> basePattern, int lengthOfList, int numbersToUse, int offsetIndex)
        {
            var returnList = new List<int>();
            var startOffset = lengthOfList * offsetIndex;
            var endOffset = (lengthOfList * offsetIndex) + lengthOfList;

            var patternLength = basePattern.Count() * numbersToUse;
            var whereToStartInPattern = startOffset % patternLength;
            var startPosIndex = 0;

            if (startOffset > 0) { 
                while (returnList.Count <= lengthOfList)
                {
                    foreach (var item in basePattern)
                    {
                        for (int i = 0; i < numbersToUse; i++)
                        {
                            if (whereToStartInPattern == 0 || startPosIndex == whereToStartInPattern - 1)
                            {                                
                                returnList.Add(item);
                                if (returnList.Count >= lengthOfList) break;
                            }
                            else
                            {
                                startPosIndex++;
                            }
                        }
                    }
                }
            }
            else
            {
                while (returnList.Count <= lengthOfList)
                {
                    foreach (var item in basePattern)
                    {
                        for (int i = 0; i < numbersToUse; i++)
                        {                            
                            returnList.Add(item);
                            if (returnList.Count >= lengthOfList) break;
                        }
                    }
                }
            }

            returnList.RemoveAt(0);

            if (returnList.Count > lengthOfList)
            {
                var numbersToRemove = returnList.Count - lengthOfList;
                returnList.RemoveRange(returnList.Count - numbersToRemove, numbersToRemove);
            }

            return returnList;
        }
    }
}
