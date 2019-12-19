using Common;
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
            int pathLength = 0;
            var collectedKeys = new List<char>();
            var collecedAllKeys = false;
            int pathIndex = 0;
            var path = new List<Node>();

            // state
            var previousGameState = new Dictionary<(int, int), string>();

            // time handling
            const long constantFPS = 5;
            const long constantMSPF = 1000 / constantFPS;
            long deltaTime = 0;
            long lastFrameTime = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            while (!collecedAllKeys)
            {
                var allKeys = gameState.Where(x => char.IsLower(char.Parse(x.Value))).ToList();

                //recieve input
                var moveVector = (0, 0);

                if (pathIndex < path.Count) { 
                    var moveToPosition = path[pathIndex];
                    moveVector = ((int)moveToPosition.Point.X - currentPosition.Item1, (int)moveToPosition.Point.Y - currentPosition.Item2);
                    currentPosition = (currentPosition.Item1 + moveVector.Item1, currentPosition.Item2 + moveVector.Item2);
                    numberOfSteps++;
                    pathIndex++;
                }

                //logic
                if (path.Count == 0)
                {
                    path = CalculatePathToNearestKey(allKeys, collectedKeys, gameState, currentPosition);
                    pathIndex++;
                }
                if (allKeys.Count == 0)
                {
                    collecedAllKeys = true;
                }
                if (char.IsLower(char.Parse(gameState[currentPosition]))) //standing on a key?
                {
                    collectedKeys.Add(char.Parse(gameState[currentPosition]));
                    gameState[currentPosition] = ".";
                    path = CalculatePathToNearestKey(allKeys, collectedKeys, gameState, currentPosition);
                    pathIndex = 1;
                }
                else if(char.IsUpper(char.Parse(gameState[currentPosition]))) //open door
                {
                    gameState[currentPosition] = ".";
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

                Console.SetCursorPosition(0, gameState.Max(x => x.Key.Item2) + 6);
                Console.WriteLine($"path length: {pathLength}");
            }
        }

        private static List<Node> CalculatePathToNearestKey(List<KeyValuePair<(int, int), string>> allKeys, List<char> collectedKeys, Dictionary<(int, int), string> gameState, (int, int) currentPosition)
        {
            var closestKey = int.MaxValue;
            var keyToMoveTo = string.Empty;
            var path = new List<Node>();
            
            foreach (var key in allKeys)
            {
                var map = GenerateMapFromGameState(gameState, currentPosition, collectedKeys);
                var isInNodes = map.Nodes.FirstOrDefault(x => x.Name == key.Value);
                if (isInNodes == null) continue;

                map.EndNode = isInNodes;
                var searchEngine = new SearchEngine(map);

                var innerPath = searchEngine.GetShortestPathAstart();
                var currentPathLength = innerPath.Count();

                if (currentPathLength < closestKey)
                {
                    closestKey = currentPathLength;
                    keyToMoveTo = key.Value;
                    path = innerPath;
                }
            }

            return path;
        }

        private static bool IsAKey(Dictionary<(int, int), string> gameState, (int, int) position)
        {
            return char.IsLower(char.Parse(gameState[position]));
        }

        private static Map GenerateMapFromGameState(Dictionary<(int, int), string> gameState, (int, int) position, List<char> collectedKeys)
        {
            var allPaths = gameState.Where(x => x.Value == "." || x.Value == "@" || char.IsLower(char.Parse(x.Value)) || collectedKeys.Contains(char.Parse(x.Value.ToLower()))).ToList();
            var map = new Map();
            var nodes = new List<Node>();

            var start = allPaths.FirstOrDefault(x => x.Key == position);
            GenerateNodes(allPaths, nodes, start);
            map.Nodes = nodes;
            var startNode = map.Nodes.FirstOrDefault(x => x.Point.X == position.Item1 && x.Point.Y == position.Item2);

            map.StartNode = startNode;

            return map;
        }

        private static void GenerateNodes(List<KeyValuePair<(int, int), string>> allPaths, List<Node> nodes, KeyValuePair<(int, int), string> currentNode)
        {
            if (!allPaths.Any(x => x.Key == currentNode.Key)) return;
            if (nodes.Any(x => x.Point.X == currentNode.Key.Item1 && x.Point.Y == currentNode.Key.Item2)) return;

            var node = new Node
            {
                Id = Guid.NewGuid(),
                Name = currentNode.Value,
                Point = new Point() { X = currentNode.Key.Item1, Y = currentNode.Key.Item2 },
            };

            nodes.Add(node);

            GenerateNodes(allPaths, nodes, allPaths.FirstOrDefault(x => x.Key == (currentNode.Key.Item1, currentNode.Key.Item2 - 1)));
            GenerateNodes(allPaths, nodes, allPaths.FirstOrDefault(x => x.Key == (currentNode.Key.Item1, currentNode.Key.Item2 + 1)));
            GenerateNodes(allPaths, nodes, allPaths.FirstOrDefault(x => x.Key == (currentNode.Key.Item1 - 1, currentNode.Key.Item2)));
            GenerateNodes(allPaths, nodes, allPaths.FirstOrDefault(x => x.Key == (currentNode.Key.Item1 + 1, currentNode.Key.Item2)));

            var nodeY1 = nodes.FirstOrDefault(x => x.Point.X == currentNode.Key.Item1 && x.Point.Y == currentNode.Key.Item2 - 1);
            if (nodeY1 != null)
            {
                var edge = new Edge
                {
                    Length = 1,
                    ConnectedNode = nodeY1
                };

                var edgeY1 = new Edge
                {
                    Length = 1,
                    ConnectedNode = node
                };

                if(!node.Connections.Any(x => x.ConnectedNode.Id == nodeY1.Id))
                    node.Connections.Add(edge);

                if (!nodeY1.Connections.Any(x => x.ConnectedNode.Id == node.Id))
                    nodeY1.Connections.Add(edgeY1);
            }

            var nodeY2 = nodes.FirstOrDefault(x => x.Point.X == currentNode.Key.Item1 && x.Point.Y == currentNode.Key.Item2 + 1);
            if (nodeY2 != null)
            {
                var edge = new Edge
                {
                    Length = 1,
                    ConnectedNode = nodeY2
                };

                var edgeY2 = new Edge
                {
                    Length = 1,
                    ConnectedNode = node
                };

                if (!node.Connections.Any(x => x.ConnectedNode.Id == nodeY2.Id))
                    node.Connections.Add(edge);

                if (!nodeY2.Connections.Any(x => x.ConnectedNode.Id == node.Id))
                    nodeY2.Connections.Add(edgeY2);
            }

            var nodeX1 = nodes.FirstOrDefault(x => x.Point.X == currentNode.Key.Item1 - 1 && x.Point.Y == currentNode.Key.Item2);
            if (nodeX1 != null)
            {
                var edge = new Edge
                {
                    Length = 1,
                    ConnectedNode = nodeX1
                };

                var edgeX1 = new Edge
                {
                    Length = 1,
                    ConnectedNode = node
                };

                if (!node.Connections.Any(x => x.ConnectedNode.Id == nodeX1.Id))
                    node.Connections.Add(edge);

                if (!nodeX1.Connections.Any(x => x.ConnectedNode.Id == node.Id))
                    nodeX1.Connections.Add(edgeX1);
            }

            var nodeX2 = nodes.FirstOrDefault(x => x.Point.X == currentNode.Key.Item1 + 1 && x.Point.Y == currentNode.Key.Item2);
            if (nodeX2 != null)
            {
                var edge = new Edge
                {
                    Length = 1,
                    ConnectedNode = nodeX2
                };

                var edgeX2 = new Edge
                {
                    Length = 1,
                    ConnectedNode = node
                };

                if (!node.Connections.Any(x => x.ConnectedNode.Id == nodeX2.Id))
                    node.Connections.Add(edge);

                if (!nodeX2.Connections.Any(x => x.ConnectedNode.Id == node.Id))
                    nodeX2.Connections.Add(edgeX2);
            }
        }

    }
}
