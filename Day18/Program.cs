using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Day18
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameState = ReadInput();
            Part1(gameState);
        }

        private static Dictionary<(int, int), string> ReadInput()
        {
            var parsedInput = FileReader.GetValuesList("./input.txt", "\r\n");
            var gameState = new Dictionary<(int, int), string>();

            int rowIndex = 0;
            foreach(var row in parsedInput)
            {
                var colIndex = 0;
                foreach(var col in row)
                {
                    gameState.Add((colIndex, rowIndex), col);
                    colIndex++;
                }

                rowIndex++;
            }

            return gameState;
        }

        private static void Part1(Dictionary<(int, int), string> gameState)
        {
            // player
            var gameStatePosition = gameState.FirstOrDefault(x => x.Value == "@");
            var startPosition = gameStatePosition.Key;
            var currentPosition = gameStatePosition.Key;
            ConsoleKeyInfo keyInfo;
            var keys = new List<string>();
            int numberOfSteps = 0;

            // state
            var previousGameState = new Dictionary<(int, int), string>();

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
                keyInfo = Console.ReadKey(true);

                //logic
                switch(keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (CanMoveToTile(gameState, (currentPosition.Item1, currentPosition.Item2 - 1), keys))
                            currentPosition.Item2--;
                        numberOfSteps++;
                        break;
                    case ConsoleKey.DownArrow:
                        if (CanMoveToTile(gameState, (currentPosition.Item1, currentPosition.Item2 + 1), keys))
                            currentPosition.Item2++;
                        numberOfSteps++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (CanMoveToTile(gameState, (currentPosition.Item1 - 1, currentPosition.Item2), keys))
                            currentPosition.Item1--;
                        numberOfSteps++;
                        break;
                    case ConsoleKey.RightArrow:
                        if (CanMoveToTile(gameState, (currentPosition.Item1 + 1, currentPosition.Item2), keys))
                            currentPosition.Item1++;
                        numberOfSteps++;
                        break;
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

                    Console.SetCursorPosition((int)gameAreaItem.Key.Item1, (int)gameAreaItem.Key.Item2);
                    Console.Write(gameAreaItem.Value);
                }

                previousGameState[currentPosition] = "P";
                Console.SetCursorPosition((int)currentPosition.Item1, (int)currentPosition.Item2);
                Console.Write("P");

                Console.SetCursorPosition(0, gameState.Max(x => x.Key.Item2) + 5);
                Console.WriteLine($"Current number of steps: {numberOfSteps}");
            }
        }

        private static bool CanMoveToTile(Dictionary<(int, int), string> gameState, (int,int) position, List<string> keys)
        {
            if(gameState[position] == "." || gameState[position] == "@")
            {
                return true;
            }
            else if (IsAKey(gameState, position))
            {
                keys.Add(gameState[position]);
                gameState[position] = ".";
                return true;
            }

            if (keys.Contains(gameState[position].ToLower()))
            {
                gameState[position] = ".";
                return true;
            }

            return false;
        }

        private static bool IsAKey(Dictionary<(int, int), string> gameState, (int, int) position)
        {
            return char.IsLower(char.Parse(gameState[position]));
        }
    }
}
