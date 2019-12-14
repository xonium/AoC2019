using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            //Part1();
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
            int outputType = 0;
            long positionX = 0;
            long positionY = 0;
            long tileType = 0;

            var gameArea = new Dictionary<(long, long), long>();

            while (output != 99)
            {
                var result = thermalEnvironmentSupervisionTerminal(longValues, 1, false, instructionPointer, relativeBase);
                output = result.Item1;
                instructionPointer = result.Item2;
                relativeBase = result.Item3;

                switch (outputType)
                {
                    case 0:
                        positionX = output;
                        outputType++;
                        break;
                    case 1:
                        positionY = output;
                        outputType++;
                        break;
                    case 2:
                        tileType = output;
                        outputType = 0;
                        AddToGameArea(tileType, positionX, positionY, gameArea);
                        break;
                }
            }

            var blockTiles = gameArea.Values.Where(x => x == 2).Count();
            Console.WriteLine($"--- part one: block tiles = {blockTiles} ---");
        }

        private static void Part2()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);
            longValues[0] = 2; //Memory address 0 represents the number of quarters that have been inserted; set it to 2 to play for free.

            long output = 0;
            int instructionPointer = 0;
            int relativeBase = 0;
            int outputType = 0;
            long positionX = 0;
            long positionY = 0;
            long tileType = 0;
            int input = 0;
            bool scoreOutput = false;
            bool runningLogicLoop = true;
            bool startPositionXSeen = false;
            bool startPositionYSeen = false;
            long score = 0;

            // state
            var previousGameState = new Dictionary<(long, long), long>();
            var gameState = new Dictionary<(long, long), long>();

            // time handling
            const long constantFPS = 5;
            const long constantMSPF = 1000 / constantFPS;
            long deltaTime = 0;
            long lastFrameTime = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (output != 99)
            {
                //recieve input
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if (keyInfo.Key == ConsoleKey.LeftArrow)
                    {
                        input = -1;
                    }
                    else if (keyInfo.Key == ConsoleKey.RightArrow)
                    {
                        input = 1;
                    }
                }

                deltaTime = constantMSPF - lastFrameTime;
                if(gameState.Count == 1008) //if gamefield havent been loaded fully. just run
                if (deltaTime > 0)
                    Thread.Sleep((int)deltaTime);

                //run logic
                runningLogicLoop = true;
                while (runningLogicLoop)
                {
                    var result = thermalEnvironmentSupervisionTerminal(longValues, input, false, instructionPointer, relativeBase);
                    output = result.Item1;
                    instructionPointer = result.Item2;
                    relativeBase = result.Item3;

                    switch (outputType)
                    {
                        case 0:
                            if (output == -1)
                            {
                                scoreOutput = true;
                            }
                            else
                            {
                                positionX = output;

                                if(startPositionXSeen) { 
                                    runningLogicLoop = false;
                                    startPositionXSeen = false;
                                }
                                else
                                    startPositionXSeen = true;
                            }

                            outputType++;
                            break;
                        case 1:
                            if (!scoreOutput)
                            {
                                positionY = output;
                                if (startPositionYSeen)
                                {
                                    runningLogicLoop = false;
                                    startPositionYSeen = false;
                                }
                                else
                                    startPositionYSeen = true;
                            }
                            outputType++;
                            break;
                        case 2:
                            if (!scoreOutput)
                            {
                                tileType = output;
                                AddToGameArea(tileType, positionX, positionY, gameState);
                            }
                            else
                            {
                                score = output;
                                scoreOutput = false;
                            }

                            outputType = 0;
                            break;
                    }
                }

                //render
                foreach(var gameAreaItem in gameState)
                {
                    if(previousGameState.ContainsKey(gameAreaItem.Key))
                    {
                        if (previousGameState[gameAreaItem.Key] == gameAreaItem.Value)
                            continue;
                    }
                    
                    previousGameState[gameAreaItem.Key] = gameAreaItem.Value;

                    var item = " ";
                    switch (gameAreaItem.Value)
                    {
                        case 0: //empty tile                            
                            item = " ";
                            break;
                        case 1: //wall tile
                            item = "X";
                            break;
                        case 2: //block tile
                            item = "B";
                            break;
                        case 3: //paddle tile
                            item = "=";
                            break;
                        case 4: //ball tile
                            item = "O";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

                lastFrameTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Restart();
            }

            stopWatch.Stop();
            Console.WriteLine($"--- RESULT {score} ---");
        }
        static void AddToGameArea(long tileType, long positionX, long positionY, Dictionary<(long, long), long> gameArea)
        {
            if(gameArea.ContainsKey((positionX, positionY)))
            {
                gameArea[(positionX, positionY)] = tileType;
            }
            else
            {
                gameArea.Add((positionX, positionY), tileType);
            }
            
        }

        static void Trace(int instructionPointer, int relativeBase, int instruction, long param1 = 0, long param2 = 0)
        {
            Console.WriteLine($"{instructionPointer}:   {instruction} {param1} {param2}");
        }

        static (long, int, int) thermalEnvironmentSupervisionTerminal(List<long> intValues, int input, bool trace = false, int instructionPointer = 0, int relativeBase = 0)
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
                    if (o.ParameterOne == Instruction.Mode.Position)
                        intValues[(int)intValues[instructionPointer + 1]] = input;
                    else if (o.ParameterOne == Instruction.Mode.Immediate)
                        intValues[instructionPointer + 1] = input;
                    else if (o.ParameterOne == Instruction.Mode.Relative)
                        intValues[relativeBase + (int)intValues[instructionPointer + 1]] = input;

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
                    return (output, instructionPointer, relativeBase);
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
                    return (99, instructionPointer, relativeBase);
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
