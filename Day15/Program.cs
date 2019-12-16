using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            Part2_2();
        }


        private static void Part1()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);
            
            long output = 0;
            int instructionPointer = 0;
            int relativeBase = 0;
            int input = 0;
            bool runningLogicLoop = true;
            long score = 0;
            List<int> inputCommands = new List<int>();
            var startPosition = (50, 50);
            var currentPosition = (50, 50);

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
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    input = 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    input = 2;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    input = 3;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    input = 4;
                }

                input = input == 0 ? 1 : input;

                inputCommands.Add(input);
                deltaTime = constantMSPF - lastFrameTime;

                //run logic
                var result = thermalEnvironmentSupervisionTerminal(longValues, input, false, instructionPointer, relativeBase);
                output = result.Item1;
                instructionPointer = result.Item2;
                relativeBase = result.Item3;

                switch (output)
                {
                    case 0: //hit a wall
                        var wallPosition = currentPosition;
                        switch (input)
                        {
                            case 1:
                                wallPosition.Item2--;
                                break;
                            case 2:
                                wallPosition.Item2++;
                                break;
                            case 3:
                                wallPosition.Item1--;
                                break;
                            case 4:
                                wallPosition.Item1++;
                                break;
                        }
                        if(!gameState.ContainsKey(wallPosition))
                        {
                            gameState.Add(wallPosition, 1);
                        }
                        break;
                    case 1: //moved in requested direction
                        switch(input)
                        {
                            case 1:
                                currentPosition.Item2--;
                                break;
                            case 2:
                                currentPosition.Item2++;
                                break;
                            case 3:
                                currentPosition.Item1--;
                                break;
                            case 4:
                                currentPosition.Item1++;
                                break;
                        }
                        if (!gameState.ContainsKey(currentPosition))
                        {
                            gameState.Add(currentPosition, 0);
                        }
                        break;
                    case 2: //oxygen system
                        switch (input)
                        {
                            case 1:
                                currentPosition.Item2--;
                                break;
                            case 2:
                                currentPosition.Item2++;
                                break;
                            case 3:
                                currentPosition.Item1--;
                                break;
                            case 4:
                                currentPosition.Item1++;
                                break;
                        }

                        if (!gameState.ContainsKey(currentPosition))
                        {
                            gameState.Add(currentPosition, 2);
                        }
                        break;
                }

                //render
                if (!gameState.ContainsKey(startPosition))
                {
                    gameState.Add(startPosition, 666);
                }

                long tile = 0;
                if(gameState.ContainsKey(currentPosition)) { 
                    tile = gameState[currentPosition];
                    gameState[currentPosition] = 9;
                }

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
                        case 0: //empty tile                            
                            item = ".";
                            break;
                        case 1: //wall tile
                            item = "X";
                            break;
                        case 2: //Oxygen tile
                            item = "O";
                            break;
                        case 9:
                            item = "P";
                            break;
                        case 666:
                            item = "S";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

                if (gameState.ContainsKey(currentPosition)) { 
                    gameState[currentPosition] = tile;
                }

                lastFrameTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Restart();
            }

            stopWatch.Stop();
            Console.WriteLine($"--- RESULT {score} ---");
        }

        private static void Part2_1()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);

            long output = 0;
            int instructionPointer = 0;
            int relativeBase = 0;
            int input = 0;
            bool runningLogicLoop = true;
            long score = 0;
            List<int> inputCommands = new List<int>();
            var startPosition = (50, 50);
            var currentPosition = (50, 50);

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

            //bot handling
            int botOrentation = 1;
            var botLocation = startPosition;
            var botListOfLocations = new List<(int, int)>();
            var random = new Random(424242);

            while (output != 99)
            {
                //print game state to file
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo saveToFileKey = Console.ReadKey(true);
                    if (saveToFileKey.Key == ConsoleKey.Q)
                    {
                        File.WriteAllText("./gamestate.txt", string.Join(",", gameState));
                    }
                }

                //recieve input
                if (output == 0)
                {                    
                    var rand = random.Next(1, 5);
                    input = rand;
                }
                else
                {
                    input = random.Next(1, 5);
                }

                input = input == 0 ? 1 : input;

                inputCommands.Add(input);
                deltaTime = constantMSPF - lastFrameTime;

                //run logic
                var result = thermalEnvironmentSupervisionTerminal(longValues, input, false, instructionPointer, relativeBase);
                output = result.Item1;
                instructionPointer = result.Item2;
                relativeBase = result.Item3;

                switch (output)
                {
                    case 0: //hit a wall
                        var wallPosition = currentPosition;
                        switch (input)
                        {
                            case 1:
                                wallPosition.Item2--;
                                break;
                            case 2:
                                wallPosition.Item2++;
                                break;
                            case 3:
                                wallPosition.Item1--;
                                break;
                            case 4:
                                wallPosition.Item1++;
                                break;
                        }
                        if (!gameState.ContainsKey(wallPosition))
                        {
                            gameState.Add(wallPosition, 1);
                        }
                        break;
                    case 1: //moved in requested direction
                        switch (input)
                        {
                            case 1:
                                currentPosition.Item2--;
                                break;
                            case 2:
                                currentPosition.Item2++;
                                break;
                            case 3:
                                currentPosition.Item1--;
                                break;
                            case 4:
                                currentPosition.Item1++;
                                break;
                        }
                        if (!gameState.ContainsKey(currentPosition))
                        {
                            gameState.Add(currentPosition, 0);
                        }
                        break;
                    case 2: //oxygen system
                        switch (input)
                        {
                            case 1:
                                currentPosition.Item2--;
                                break;
                            case 2:
                                currentPosition.Item2++;
                                break;
                            case 3:
                                currentPosition.Item1--;
                                break;
                            case 4:
                                currentPosition.Item1++;
                                break;
                        }

                        if (!gameState.ContainsKey(currentPosition))
                        {
                            gameState.Add(currentPosition, 2);
                        }
                        break;
                }

                //render
                if (!gameState.ContainsKey(startPosition))
                {
                    gameState.Add(startPosition, 666);
                }

                long tile = 0;
                if (gameState.ContainsKey(currentPosition))
                {
                    tile = gameState[currentPosition];
                    gameState[currentPosition] = 9;
                }

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
                        case 0: //empty tile                            
                            item = ".";
                            break;
                        case 1: //wall tile
                            item = "X";
                            break;
                        case 2: //Oxygen tile
                            item = "O";
                            break;
                        case 9:
                            item = "P";
                            break;
                        case 666:
                            item = "S";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

                if (gameState.ContainsKey(currentPosition))
                {
                    gameState[currentPosition] = tile;
                }

                lastFrameTime = stopWatch.ElapsedMilliseconds;
                stopWatch.Restart();
            }

            stopWatch.Stop();
            Console.WriteLine($"--- RESULT {score} ---");
        }

        private static void Part2_2()
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 10).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);
            
            //oxygen handling
            var oxygenPositions = new List<(long, long)>();
            var oxygenFillMinutes = 0;

            // state
            var previousGameState = new Dictionary<(long, long), long>();
            var gameState = new Dictionary<(long, long), long>();

            var gameStateStrings = FileReader.GetValuesString("./gamestate.txt", "],");
            foreach (var stateString in gameStateStrings)
            {
                try
                {
                    Regex regexObj = new Regex(@"\d+");
                    long x = 0;
                    long y = 0;
                    long value = 0;
                    int count = 0;
                    bool foundOxygenStart = false;
                    Match matchResults = regexObj.Match(stateString);
                    while (matchResults.Success)
                    {
                        // matched text: matchResults.Value
                        // match start: matchResults.Index
                        // match length: matchResults.Length
                        switch (count)
                        {
                            case 0:
                                x = long.Parse(matchResults.Value);
                                break;
                            case 1:
                                y = long.Parse(matchResults.Value);
                                break;
                            case 2:
                                value = long.Parse(matchResults.Value);
                                if(value == 2)
                                {
                                    foundOxygenStart = true;
                                }
                                break;
                        }

                        matchResults = matchResults.NextMatch();
                        count++;
                    }

                    if(!gameState.ContainsKey((x,y)))
                        gameState.Add((x, y), value);

                    if(foundOxygenStart)
                    {
                        oxygenPositions.Add((x, y));
                        foundOxygenStart = false;
                    }
                }
                catch (ArgumentException ex)
                {
                    // Syntax error in the regular expression
                }
            }

            // time handling
            const long constantFPS = 5;
            const long constantMSPF = 1000 / constantFPS;
            long deltaTime = 0;
            long lastFrameTime = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            while (true)
            {
                //recieve input

                //logic
                var newOxygenPositions = new List<(long, long)>();
                foreach (var oxygenPosition in oxygenPositions)
                {
                    if(gameState[(oxygenPosition.Item1 + 1, oxygenPosition.Item2)] == 0)
                    {
                        gameState[(oxygenPosition.Item1 + 1, oxygenPosition.Item2)] = 2;
                        newOxygenPositions.Add((oxygenPosition.Item1 + 1, oxygenPosition.Item2));
                    }
                    if (gameState[(oxygenPosition.Item1 - 1, oxygenPosition.Item2)] == 0)
                    {
                        gameState[(oxygenPosition.Item1 - 1, oxygenPosition.Item2)] = 2;
                        newOxygenPositions.Add((oxygenPosition.Item1 - 1, oxygenPosition.Item2));
                    }
                    if (gameState[(oxygenPosition.Item1, oxygenPosition.Item2 + 1)] == 0)
                    {
                        gameState[(oxygenPosition.Item1, oxygenPosition.Item2 + 1)] = 2;
                        newOxygenPositions.Add((oxygenPosition.Item1, oxygenPosition.Item2 + 1));
                    }
                    if (gameState[(oxygenPosition.Item1, oxygenPosition.Item2 - 1)] == 0)
                    {
                        gameState[(oxygenPosition.Item1, oxygenPosition.Item2 - 1)] = 2;
                        newOxygenPositions.Add((oxygenPosition.Item1, oxygenPosition.Item2 - 1));
                    }
                }

                oxygenPositions.AddRange(newOxygenPositions);

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
                        case 0: //empty tile                            
                            item = ".";
                            break;
                        case 1: //wall tile
                            item = "X";
                            break;
                        case 2: //Oxygen tile
                            item = "O";
                            break;
                        case 9: 
                            item = "P";
                            break;
                        case 666:
                            item = "S";
                            break;
                    }

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(item);
                }

                oxygenFillMinutes++;
            }
        }


        static void AddToGameArea(long tileType, long positionX, long positionY, Dictionary<(long, long), long> gameArea)
        {
            if (gameArea.ContainsKey((positionX, positionY)))
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
