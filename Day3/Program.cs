using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var wires = FileReader.GetValues("./input.txt", ",", "\r\n");

            var length = largestWireLength(wires);
            var turns = largestNumberOfTurns(wires);
            var grid = generateGrid(length, turns);

            var start = (length * turns) / 2;

            generateWiresInGrid(grid, wires, start);
            var interSections = findInterSections(grid);
            int lengthToOrigin = closestIntersection(interSections, start);

            Console.WriteLine(lengthToOrigin);
        }

        private static int closestIntersection(List<List<int>> interSections, int start)
        {
            var closest = int.MaxValue;
            foreach(var intersection in interSections)
            {
                var diffy = Math.Abs(intersection[0] - start);
                var diffx = Math.Abs(intersection[1] - start);

                var totalLength = diffx + diffy;

                if(totalLength < closest)
                {
                    closest = totalLength;
                }
            }

            return closest;
        }

        private static List<List<int>> findInterSections(List<List<string>> grid)
        {
            var returnIntersections = new List<List<int>>();
            for (int i = 0; i < grid.Count; i++)
            {
                for (int p = 0; p < grid[i].Count; p++)
                {
                    if(grid[i][p] == "X")
                    {
                        returnIntersections.Add(new List<int>() { i, p });
                    }
                }
            }

            return returnIntersections;
        }

        private static void generateWiresInGrid(List<List<string>> grid, List<List<string>> wires, int start)
        {

            foreach(var wire in wires)
            {
                var currentPosition = new List<int> { start, start };

                foreach(var length in wire)
                {
                    var lengthEnum = length.Skip(1);
                    var lengthOfWire = int.Parse(string.Join("", lengthEnum));
                    var direction = length[0];

                    switch(direction)
                    {
                        case 'R':
                            for (int i = 0; i < lengthOfWire; i++)
                            {
                                currentPosition[1] = currentPosition[1] + 1;
                                generateWire(grid, currentPosition, "-");
                            }
                            break;
                        case 'L':
                            for (int i = 0; i < lengthOfWire; i++)
                            {
                                currentPosition[1] = currentPosition[1] - 1;
                                generateWire(grid, currentPosition, "-");
                            }
                            break;
                        case 'U':
                            for (int i = 0; i < lengthOfWire; i++)
                            {
                                currentPosition[0] = currentPosition[0] - 1;
                                generateWire(grid, currentPosition, "|");
                            }
                            break;
                        case 'D':
                            for (int i = 0; i < lengthOfWire; i++)
                            {
                                currentPosition[0] = currentPosition[0] + 1;
                                generateWire(grid, currentPosition, "|");
                            }
                            break;
                    }
                }
            }
        }

        static void generateWire(List<List<string>> grid, List<int> position, string direction)
        {
            var valueInGrid = grid[position[0]][position[1]];

            if (valueInGrid == ".")
            {
                grid[position[0]][position[1]] = direction;
            }
            else if(valueInGrid != "o")
            {
                grid[position[0]][position[1]] = "X"; // intersection
            }   
        }

        static List<List<string>> generateGrid(int length, int turns)
        {
            var maximumSize = length * turns;
            var grid = new List<List<string>>();

            for (int i = 0; i < maximumSize; i++)
            {
                grid.Add(new List<string>());
                grid[i].AddRange(new string('.', maximumSize).Select(x => x.ToString()));
            }

            var middle = maximumSize / 2;

            grid[middle][middle] = "o";
            return grid;
        }

        static int largestWireLength(List<List<string>> wires)
        {
            var maximumLength = 0;
            foreach (var wire in wires) { 
                foreach(var lengths in wire)
                {
                    var lengthEnum = lengths.Skip(1);
                    var length = int.Parse(string.Join("", lengthEnum));
                    if(length > maximumLength)
                    {
                        maximumLength = length;
                    }
                }
            }

            return maximumLength;
        }

        static int largestNumberOfTurns(List<List<string>> wires)
        {
            var maximumTurns = 0;
            foreach (var wire in wires)
            {
                if(wire.Count > maximumTurns)
                {
                    maximumTurns = wire.Count;
                }
            }

            return maximumTurns;
        }
    }
}
