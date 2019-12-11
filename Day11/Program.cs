using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    class Program
    {        
        static void Main(string[] args)
        {
            var longValues = FileReader.GetValuesLong("./input.txt", ",");
            var addingMemoryToProgram = Enumerable.Repeat(0, longValues.Count * 50).Select((x) => long.Parse(x.ToString()));
            longValues.AddRange(addingMemoryToProgram);

            var robotoverview = FileReader.GetValuesList("./robotoverview.txt", "\r\n");

            var output = 0f;
            var instructionPointer = 0;
            var relativeBase = 0;
            var robotCurrentPosition = (0, 0);
            var robotCurrentRotation = Direction.UP;
            var currentlySees = 0;
            var paintToggle = true;
            var paintDictionary = new Dictionary<(int, int), int>();

            while (output != -1)
            {
                currentlySees = paintDictionary.GetValueOrDefault(robotCurrentPosition);

                var result = IntCode.ThermalEnvironmentSupervisionTerminal(longValues, currentlySees, trace: false, instructionPointer, relativeBase);
                if (paintToggle) { 
                    output = result.Item1;

                    //paint white
                    if (output == 1) {
                        if (paintDictionary.ContainsKey(robotCurrentPosition))
                            paintDictionary[robotCurrentPosition] = 1;
                        else
                            paintDictionary.Add(robotCurrentPosition, 1);
                    }
                    //paint black
                    else if (output == 0)
                    {
                        if (paintDictionary.ContainsKey(robotCurrentPosition))
                            paintDictionary[robotCurrentPosition] = 0;
                        else
                            paintDictionary.Add(robotCurrentPosition, 0);
                    }
                    else
                    {
                        Console.WriteLine("SOMETHING IS WROOOONG");
                    }
                }
                else
                {
                    //right 90 degrees
                    if (result.Item1 == 1) 
                    {
                        switch(robotCurrentRotation)
                        {
                            case Direction.UP:
                                robotCurrentRotation = Direction.RIGHT;
                                robotCurrentPosition = (robotCurrentPosition.Item1 + 1, robotCurrentPosition.Item2);
                                break;
                            case Direction.DOWN:
                                robotCurrentRotation = Direction.LEFT;
                                robotCurrentPosition = (robotCurrentPosition.Item1 - 1, robotCurrentPosition.Item2);
                                break;
                            case Direction.LEFT:
                                robotCurrentRotation = Direction.UP;
                                robotCurrentPosition = (robotCurrentPosition.Item1, robotCurrentPosition.Item2 - 1);
                                break;
                            case Direction.RIGHT:
                                robotCurrentRotation = Direction.DOWN;
                                robotCurrentPosition = (robotCurrentPosition.Item1, robotCurrentPosition.Item2 + 1);
                                break;
                        }
                    }
                    //left 90 degrees
                    else if (result.Item1 == 0)
                    {
                        switch (robotCurrentRotation)
                        {
                            case Direction.UP:
                                robotCurrentRotation = Direction.LEFT;
                                robotCurrentPosition = (robotCurrentPosition.Item1 - 1, robotCurrentPosition.Item2);
                                break;
                            case Direction.DOWN:
                                robotCurrentRotation = Direction.RIGHT;
                                robotCurrentPosition = (robotCurrentPosition.Item1 + 1, robotCurrentPosition.Item2);
                                break;
                            case Direction.LEFT:
                                robotCurrentRotation = Direction.DOWN;
                                robotCurrentPosition = (robotCurrentPosition.Item1, robotCurrentPosition.Item2 + 1);
                                break;
                            case Direction.RIGHT:
                                robotCurrentRotation = Direction.UP;
                                robotCurrentPosition = (robotCurrentPosition.Item1, robotCurrentPosition.Item2 - 1);
                                break;
                        }
                    }
                }

                instructionPointer = result.Item2;
                relativeBase = result.Item3;
                paintToggle = !paintToggle;
            }

            Console.WriteLine($"DOOONE panels painted: {paintDictionary.Count()}");
        }

        static void Paint(List<List<string>> robotoverview, (int,int) robotCurrentPosition)
        {
            foreach(var row in robotoverview)
            {
                Console.WriteLine(string.Join("", row));
            }
        }
    }
}
