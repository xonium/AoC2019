using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            var maxResult = 0;
            for (int param1 = 0; param1 < 5; param1++)
            {
                for (int param2 = 0; param2 < 5; param2++)
                {
                    if (param2 == param1)
                        continue;

                    for (int param3 = 0; param3 < 5; param3++)
                    {
                        if (param3 == param2 || param3 == param1)
                            continue;

                        for (int param4 = 0; param4 < 5; param4++)
                        {
                            if (param4 == param3 || param4 == param2 || param4 == param1)
                                continue;

                            for (int param5 = 0; param5 < 5; param5++)
                            {
                                if (param5 == param4 || param5 == param3 || param5 == param2 || param5 == param1)
                                    continue;

                                var result = AmplifierCombination(param1, param2, param3, param4, param5);

                                if (maxResult < result)
                                {
                                    Console.WriteLine($"{result} -- Combination: {param1}, {param2}, {param3}, {param4}, {param5} --");
                                    maxResult = result;
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"-- maxResult -- {maxResult}");

        }

        static int AmplifierCombination(int param1, int param2, int param3, int param4, int param5)
        {
            var result1 = RunAmplifier(param1, 0);
            var result2 = RunAmplifier(param2, result1);
            var result3 = RunAmplifier(param3, result2);
            var result4 = RunAmplifier(param4, result3);
            var result5 = RunAmplifier(param5, result4);

            return result5;
        }

        static int RunAmplifier(int phaseSetting, int outputFromPreviousAmplifier)
        {
            return thermalEnvironmentSupervisionTerminal(FileReader.GetValues("./input.txt", ","), phaseSetting, outputFromPreviousAmplifier);
        }

        static int thermalEnvironmentSupervisionTerminal(List<int> intValues, int phaseSetting, int outputFromPreviousAmplifier)
        {
            var output = 0;
            int instructionPointer = 0;
            var exit = false;
            var inputNumber = 0;

            while (!exit)
            {
                if (instructionPointer > intValues.Count)
                {
                    break;
                }

                var o = new Instruction(intValues[instructionPointer]);
                if (o.Code == 1)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];

                    var result = param1 + param2;

                    if (o.ParameterThree == Instruction.Mode.Position)
                    {
                        intValues[intValues[instructionPointer + 3]] = result;
                    }
                    else
                    {
                        intValues[instructionPointer + 3] = result;
                    }

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 2)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];

                    var result = param1 * param2;

                    if (o.ParameterThree == Instruction.Mode.Position)
                    {
                        intValues[intValues[instructionPointer + 3]] = result;
                    }
                    else
                    {
                        intValues[instructionPointer + 3] = result;
                    }

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 3)
                {
                    if (o.ParameterOne == Instruction.Mode.Position)
                    {
                        if(inputNumber == 0) { 
                            intValues[intValues[instructionPointer + 1]] = phaseSetting;
                        }
                        else
                        {
                            intValues[intValues[instructionPointer + 1]] = outputFromPreviousAmplifier;
                        }
                    }
                    else
                    {
                        if(inputNumber == 0)
                        {
                            intValues[instructionPointer + 1] = phaseSetting;
                        }
                        else
                        {
                            intValues[instructionPointer + 1] = outputFromPreviousAmplifier;
                        }
                    }

                    inputNumber++;
                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 4)
                {
                    if (o.ParameterOne == Instruction.Mode.Position)
                    {
                        output = intValues[intValues[instructionPointer + 1]];
                    }
                    else
                    {
                        output = intValues[instructionPointer + 1];
                    }
                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 5)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];
                    if (param1 != 0)
                    {
                        instructionPointer = param2;
                    }
                    else
                    {
                        instructionPointer = instructionPointer + 3;
                    }
                }
                else if (o.Code == 6)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];
                    if (param1 == 0)
                    {
                        instructionPointer = param2;
                    }
                    else
                    {
                        instructionPointer = instructionPointer + 3;
                    }
                }
                else if (o.Code == 7)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];

                    if (param1 < param2)
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                        {
                            intValues[intValues[instructionPointer + 3]] = 1;
                        }
                        else
                        {
                            intValues[intValues[instructionPointer + 3]] = 1;
                        }
                    }
                    else
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                        {
                            intValues[intValues[instructionPointer + 3]] = 0;
                        }
                        else
                        {
                            intValues[intValues[instructionPointer + 3]] = 0;
                        }
                    }

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 8)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];

                    if (param1 == param2)
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                        {
                            intValues[intValues[instructionPointer + 3]] = 1;
                        }
                        else
                        {
                            intValues[intValues[instructionPointer + 3]] = 1;
                        }
                    }
                    else
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                        {
                            intValues[intValues[instructionPointer + 3]] = 0;
                        }
                        else
                        {
                            intValues[intValues[instructionPointer + 3]] = 0;
                        }
                    }

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 99)
                {
                    exit = true;
                }
            }

            return output;
        }
    }

    internal class Instruction
    {
        public enum Mode
        {
            Position = 0,
            Immediate = 1
        }

        public int Code { get; set; }
        public Mode ParameterOne { get; set; }
        public Mode ParameterTwo { get; set; }
        public Mode ParameterThree { get; set; }

        public Instruction(int instruction)
        {
            var instString = instruction.ToString();

            if (instString.Length == 1)
            {
                Code = instruction;
            }
            else if (instString.Length > 1)
            {
                if (instString.Length - 2 > -1)
                {
                    var codeString = instString.Substring(instString.Length - 2, 2);
                    Code = int.Parse(codeString);
                }

                if (instString.Length - 3 > -1)
                {
                    var parameterOneString = instString.Substring(instString.Length - 3, 1);
                    ParameterOne = (Mode)Enum.Parse(typeof(Mode), parameterOneString);
                }

                if (instString.Length - 4 > -1)
                {
                    var parameterTwoString = instString.Substring(instString.Length - 4, 1);
                    ParameterTwo = (Mode)Enum.Parse(typeof(Mode), parameterTwoString);
                }

                if (instString.Length - 5 > -1)
                {
                    var parameterThreeString = instString.Substring(instString.Length - 5, 1);
                    ParameterThree = (Mode)Enum.Parse(typeof(Mode), parameterThreeString);
                }
            }
        }
    }
}
