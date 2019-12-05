using PuzzleInputParser;
using System;
using System.Collections.Generic;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var intValues = FileReader.GetValues("./input.txt", ",");

            Console.WriteLine(thermalEnvironmentSupervisionTerminal(intValues, 5));
        }

        static int thermalEnvironmentSupervisionTerminal(List<int> intValues, int input)
        {
            int instructionPointer = 0;
            var exit = false;

            while (!exit)
            {
                if (instructionPointer > intValues.Count) { 
                    break;
                }

                var o = new Instruction(intValues[instructionPointer]);
                if (o.Code == 1)
                {
                    var param1 = o.ParameterOne == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 1]] : intValues[instructionPointer + 1];
                    var param2 = o.ParameterTwo == Instruction.Mode.Position ? intValues[intValues[instructionPointer + 2]] : intValues[instructionPointer + 2];

                    var result = param1 + param2;

                    if (o.ParameterThree == Instruction.Mode.Position) {
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
                        intValues[intValues[instructionPointer + 1]] = input;
                    }
                    else
                    {
                        intValues[instructionPointer + 1] = input;
                    }

                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 4)
                {
                    var result = 0;
                    if (o.ParameterOne == Instruction.Mode.Position)
                    {
                        result = intValues[intValues[instructionPointer + 1]];
                    }
                    else
                    {
                        result = intValues[instructionPointer + 1];
                    }

                    Console.WriteLine($"output: {result}");
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

                    if(param1 < param2)
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

            return intValues[0];
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
            else if(instString.Length > 1)
            {
                if(instString.Length - 2 > -1)
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
