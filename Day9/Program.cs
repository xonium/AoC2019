using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);

            int instructionPointer = 0;
            int relativeBase = 0;

            var result = thermalEnvironmentSupervisionTerminal(longValues, 1, true, instructionPointer, relativeBase);
        }

        static void Trace(int instructionPointer, int relativeBase, int instruction, long param1=0, long param2=0)
        {
            Console.WriteLine($"{instructionPointer} {relativeBase} {instruction} {param1} {param2}");
        }

        static (long, int, int) thermalEnvironmentSupervisionTerminal(List<long> intValues, int input, bool trace=false, int instructionPointer=0, int relativeBase=0)
        {
            var exit = false;
            var output = long.Parse("0");

            while (!exit)
            {
                if (instructionPointer > intValues.Count)
                {
                    break;
                }

                if (trace) Trace(instructionPointer, relativeBase, (int)intValues[instructionPointer], (int)intValues[instructionPointer+1], (int)intValues[instructionPointer+2]);
                var o = new Instruction((int)intValues[instructionPointer]);
                if (o.Code == 1)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    var result = param1 + param2;

                    if (o.ParameterThree == Instruction.Mode.Position)
                        intValues[(int)intValues[instructionPointer + 3]] = result;
                    else if (o.ParameterThree == Instruction.Mode.Immediate)
                        intValues[instructionPointer + 3] = result;
                    else if (o.ParameterThree == Instruction.Mode.Relative)
                        intValues[relativeBase + (int)intValues[instructionPointer + 3]] = result;

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 2)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    var result = param1 * param2;

                    if (o.ParameterThree == Instruction.Mode.Position)
                        intValues[(int)intValues[instructionPointer + 3]] = result;
                    else if (o.ParameterThree == Instruction.Mode.Immediate)
                        intValues[instructionPointer + 3] = result;
                    else if (o.ParameterThree == Instruction.Mode.Relative)
                        intValues[relativeBase + (int)intValues[instructionPointer + 3]] = result;

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 3)
                {
                    if (o.ParameterOne == Instruction.Mode.Position)
                        intValues[(int)intValues[instructionPointer + 1]] = input;
                    else if(o.ParameterOne == Instruction.Mode.Immediate)
                        intValues[instructionPointer + 1] = input;
                    else if (o.ParameterThree == Instruction.Mode.Relative)
                        intValues[relativeBase + (int)intValues[instructionPointer + 3]] = input;

                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 4)
                {
                    if (o.ParameterOne == Instruction.Mode.Position)
                        output = intValues[(int)intValues[instructionPointer + 1]];
                    else if(o.ParameterOne == Instruction.Mode.Immediate)
                        output = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        output = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    Console.WriteLine($"output: {output}");
                    instructionPointer = instructionPointer + 2;
                    //return (output, instructionPointer, relativeBase);
                }
                // jump-if-true
                else if (o.Code == 5)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    if (param1 != 0)
                    {
                        instructionPointer = (int)param2;
                    }
                    else
                    {
                        instructionPointer = instructionPointer + 3;
                    }
                }
                // jump-if-false
                else if (o.Code == 6)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    if (param1 == 0)
                    {
                        instructionPointer = (int)param2;
                    }
                    else
                    {
                        instructionPointer = instructionPointer + 3;
                    }
                }
                // less than
                else if (o.Code == 7)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    if (param1 < param2)
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                            intValues[(int)intValues[instructionPointer + 3]] = long.Parse(1.ToString());
                        else if(o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 1;
                        else if (o.ParameterThree == Instruction.Mode.Relative)
                            intValues[relativeBase + (int)intValues[instructionPointer + 3]] = 1;
                    }
                    else
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                            intValues[(int)intValues[instructionPointer + 3]] = 0;
                        else if(o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 0;
                        else if(o.ParameterThree == Instruction.Mode.Relative)
                            intValues[relativeBase + (int)intValues[instructionPointer + 3]] = 1;
                    }

                    instructionPointer = instructionPointer + 4;
                }
                // equals
                else if (o.Code == 8)
                {
                    var param1 = long.Parse("0");
                    var param2 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    if (o.ParameterTwo == Instruction.Mode.Position)
                        param2 = intValues[(int)intValues[instructionPointer + 2]];
                    else if (o.ParameterTwo == Instruction.Mode.Immediate)
                        param2 = intValues[instructionPointer + 2];
                    else if (o.ParameterTwo == Instruction.Mode.Relative)
                        param2 = intValues[relativeBase + (int)intValues[instructionPointer + 2]];

                    if (param1 == param2)
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                            intValues[(int)intValues[instructionPointer + 3]] = 1;
                        else if(o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 1;
                        else if(o.ParameterThree == Instruction.Mode.Relative)
                            intValues[relativeBase + (int)intValues[instructionPointer + 3]] = 1;
                    }
                    else
                    {
                        if (o.ParameterThree == Instruction.Mode.Position)
                            intValues[(int)intValues[instructionPointer + 3]] = 0;
                        else if (o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 0;
                        else if (o.ParameterThree == Instruction.Mode.Relative)
                            intValues[relativeBase + (int)intValues[instructionPointer + 3]] = 0;
                    }

                    instructionPointer = instructionPointer + 4;
                }
                else if (o.Code == 9)
                {
                    var param1 = long.Parse("0");

                    if (o.ParameterOne == Instruction.Mode.Position)
                        param1 = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        param1 = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        param1 = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    relativeBase = relativeBase + (int)param1;
                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 99)
                {
                    exit = true;
                }
            }

            return (output, instructionPointer, relativeBase);
        }

        internal class Instruction
        {
            public enum Mode
            {
                Position = 0,
                Immediate = 1,
                Relative = 2
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


}
