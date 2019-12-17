using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Day17
{
    class Program
    {
        static void Main(string[] args)
        {
            Part2();
        }

        private static void Part1()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);

            long output = 0;
            int instructionPointer = 0;
            int relativeBase = 0;
            List<int> input = new List<int> { 0 };

            var startPosition = (0, 0);
            var currentPosition = (0, 0);

            // state
            var previousGameState = new Dictionary<(long, long), long>();
            var gameState = new Dictionary<(long, long), long>();
            var cameraRunning = true;

            // time handling
            const long constantFPS = 5;
            const long constantMSPF = 1000 / constantFPS;
            long deltaTime = 0;
            long lastFrameTime = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            Console.BufferWidth = 200;

            while (cameraRunning)
            {
                //recieve input

                //logic
                var result = thermalEnvironmentSupervisionTerminal(longValues, input, false, instructionPointer, relativeBase);
                output = result.Item1;
                instructionPointer = result.Item2;
                relativeBase = result.Item3;

                if (output == 99) cameraRunning = false;

                if (output != 10) { 
                    if (!gameState.ContainsKey(currentPosition))
                        gameState.Add(currentPosition, output);
                    else
                        gameState[currentPosition] = output;

                    currentPosition.Item1++;
                }
                else
                {
                    currentPosition.Item1 = startPosition.Item1;
                    currentPosition.Item2++;
                }

                if(currentPosition.Item1 > 120 && currentPosition.Item2 > 65)
                {
                    currentPosition = startPosition;
                }

                var maxX = gameState.Keys.Max(x => x.Item1);
                var maxY = gameState.Keys.Max(x => x.Item2);

                for (long x = 0; x < maxX; x++)
                {
                    for (long y = 0; y < maxY; y++)
                    {
                        if (gameState.ContainsKey((x, y)) && gameState[(x, y)] == 35)
                        {
                            if (!gameState.ContainsKey((x + 1, y)) || gameState[(x + 1, y)] != 35)
                            {
                                continue;
                            }
                            if (!gameState.ContainsKey((x - 1, y)) || gameState[(x - 1, y)] != 35)
                            {
                                continue;
                            }
                            if (!gameState.ContainsKey((x, y + 1)) || gameState[(x, y + 1)] != 35)
                            {
                                continue;
                            }
                            if (!gameState.ContainsKey((x, y - 1)) || gameState[(x, y - 1)] != 35)
                            {
                                continue;
                            }

                            gameState[(x, y)] = 55;
                        }
                    }
                }

                //render
                foreach (var gameAreaItem in gameState)
                {
                    if (previousGameState.ContainsKey(gameAreaItem.Key))
                    {
                        if (previousGameState[gameAreaItem.Key] == gameAreaItem.Value)
                            continue;
                    }

                    previousGameState[gameAreaItem.Key] = gameAreaItem.Value;

                    var item = " ";

                    switch (gameAreaItem.Value)
                    {
                        case 46: //empty tile                            
                            item = ".";
                            break;
                        case 35: //wall tile
                            item = "#";
                            break;
                        case 10: //new line
                            item = "N";
                            break;
                        case 55:
                            item = "O";
                            break;
                        case 94:
                            item = "^";
                            break;
                        case 60:
                            item = "<";
                            break;
                        case 62:
                            item = ">";
                            break;
                        case 118:
                            item = "v";
                            break;
                        case 666:
                            item = "S";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

            }

            var intersections = gameState.Where(x => x.Value == 55).ToList();

            var resultList = new List<long>();
            foreach(var intersection in intersections)
            {
                resultList.Add(intersection.Key.Item1 * intersection.Key.Item2);
            }

            Console.WriteLine($"The sum of all intersections is {resultList.Sum()}");
        }

        private static void Part2()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);
            longValues[0] = 2; // Force the vacuum robot to wake up by changing the value in your ASCII program at address 0 from 1 to 2

            long output = 0;
            int instructionPointer = 0;
            int inputPointer = 0;
            int relativeBase = 0;
            List<int> input = new List<int>() { 0 };

            var startPosition = (0, 0);
            var currentPosition = (0, 0);

            // bot handling
            var botPosition = (0, 0);
            var botPositionVisited = new List<(int, int)>();
            var mainMovementRoutines = Encoding.ASCII.GetBytes("A,B,C,A,B,C\n");
            var functionARoutines    = Encoding.ASCII.GetBytes("L,L,L,L,L\n");
            var functionBRoutines    = Encoding.ASCII.GetBytes("L\n");
            var functionCRoutines    = Encoding.ASCII.GetBytes("L\n");
            var videoFeed            = Encoding.ASCII.GetBytes("y\n");
            var end                  = Encoding.ASCII.GetBytes("\n\n");

            // state
            var previousGameState = new Dictionary<(long, long), long>();
            var gameState = new Dictionary<(long, long), long>();
            var cameraRunning = true;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var inputList = new List<int>();
            inputList.AddRange(mainMovementRoutines.Select(x => (int)x).ToList());
            inputList.AddRange(functionARoutines.Select(x => (int)x).ToList());
            inputList.AddRange(functionBRoutines.Select(x => (int)x).ToList());
            inputList.AddRange(functionCRoutines.Select(x => (int)x).ToList());
            inputList.AddRange(videoFeed.Select(x => (int)x).ToList());
            inputList.AddRange(end.Select(x => (int)x).ToList());

            while (true)
            {
                //logic
                var result = thermalEnvironmentSupervisionTerminal(longValues, inputList, false, instructionPointer, relativeBase, inputPointer);
                output = result.Item1;
                instructionPointer = result.Item2;
                relativeBase = result.Item3;
                inputPointer = result.Item4;

                if (output == 99) cameraRunning = false;

                if (output == 46 || output == 35 || output == 94 || output == 60 || output == 62 || output == 118)
                {
                    if (!gameState.ContainsKey(currentPosition))
                        gameState.Add(currentPosition, output);
                    else
                        gameState[currentPosition] = output;

                    currentPosition.Item1++;
                }
                else if (output == 10)
                {
                    currentPosition.Item1 = startPosition.Item1;
                    currentPosition.Item2++;
                }
                if ((output == 46 || output == 35 || output == 94 || output == 60 || output == 62 || output == 118) && currentPosition.Item2 > 45)
                {
                    currentPosition = startPosition;
                }

                //render
                foreach (var gameAreaItem in gameState)
                {
                    if (previousGameState.ContainsKey(gameAreaItem.Key))
                    {
                        if (previousGameState[gameAreaItem.Key] == gameAreaItem.Value)
                            continue;
                    }

                    previousGameState[gameAreaItem.Key] = gameAreaItem.Value;

                    var item = " ";

                    switch (gameAreaItem.Value)
                    {
                        case 46: //empty tile                            
                            item = ".";
                            break;
                        case 35: //wall tile
                            item = "#";
                            break;
                        case 10: //new line
                            item = "N";
                            break;
                        case 55:
                            item = "O";
                            break;
                        case 94:
                            item = "^";
                            break;
                        case 60:
                            item = "<";
                            break;
                        case 62:
                            item = ">";
                            break;
                        case 118:
                            item = "v";
                            break;
                        case 666:
                            item = "S";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

            }

        }

        static void Trace(int instructionPointer, int relativeBase, int instruction, long param1 = 0, long param2 = 0)
        {
            Console.WriteLine($"{instructionPointer}:   {instruction} {param1} {param2}");
        }

        static (long, int, int, int) thermalEnvironmentSupervisionTerminal(List<long> intValues, List<int> input, bool trace = false, int instructionPointer = 0, int relativeBase = 0, int inputPointer = 0)
        {
            var exit = false;
            var output = long.Parse("0");

            while (!exit)
            {
                if (instructionPointer > intValues.Count)
                {
                    break;
                }

                if (trace) Trace(instructionPointer, relativeBase, (int)intValues[instructionPointer], (int)intValues[instructionPointer + 1], (int)intValues[instructionPointer + 2]);
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
                    var currentInputValue = 0;
                    if(input != null && inputPointer < input.Count)
                    {
                        currentInputValue = input[inputPointer];                        
                    }

                    if (o.ParameterOne == Instruction.Mode.Position)
                        intValues[(int)intValues[instructionPointer + 1]] = currentInputValue;
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        intValues[instructionPointer + 1] = currentInputValue;
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        intValues[relativeBase + (int)intValues[instructionPointer + 1]] = currentInputValue;

                    inputPointer++;
                    instructionPointer = instructionPointer + 2;
                }
                else if (o.Code == 4)
                {
                    if (o.ParameterOne == Instruction.Mode.Position)
                        output = intValues[(int)intValues[instructionPointer + 1]];
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        output = intValues[instructionPointer + 1];
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        output = intValues[relativeBase + (int)intValues[instructionPointer + 1]];

                    instructionPointer = instructionPointer + 2;
                    return (output, instructionPointer, relativeBase, inputPointer);
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
                            intValues[(int)intValues[instructionPointer + 3]] = 1;
                        else if (o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 1;
                        else if (o.ParameterThree == Instruction.Mode.Relative)
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
                        else if (o.ParameterThree == Instruction.Mode.Immediate)
                            intValues[instructionPointer + 3] = 1;
                        else if (o.ParameterThree == Instruction.Mode.Relative)
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
                    return (99, instructionPointer, relativeBase, inputPointer);
                }
            }

            return (output, instructionPointer, relativeBase, inputPointer);
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
