using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            //part 1
            var space = FileReader.GetValuesList("./input.txt", "\r\n");
            
            var asteroidLocations = GetLocationOfAsteroids(space);

            CalculateNearbyAsteroidsFromLocations(asteroidLocations, space);
            var bestLocation = GetBestLocation(space);

            Console.WriteLine($"-- Best location spots: {bestLocation.Item1} asteroids at location x: {bestLocation.Item2},y: {bestLocation.Item3}  --");

            //part 2
            var space2 = FileReader.GetValuesList("./input.txt", "\r\n");

            space2[bestLocation.Item3][bestLocation.Item2] = "X";

            PrintSpace(space2);
            VaporizeAsteroids(space2, (bestLocation.Item2, bestLocation.Item3));
        }

        private static void VaporizeAsteroids(List<List<string>> space, (int, int) baseLocation)
        {
            var asteroidLocations = GetLocationOfAsteroids(space);
            var orderedAngles = CalculateAngles(baseLocation, asteroidLocations).OrderBy(x => x.Item1).ThenBy(x => x.Item2);
        }

        private static (int, int, int) GetBestLocation(List<List<string>> space)
        {
            var maxResult = 0;
            var bestLocationX = 0;
            var bestLocationY = 0;
            var rowIndex = 0;
            var colIndex = 0;

            foreach(var row in space)
            {
                colIndex = 0;
                foreach(var col in row)
                {
                    if(int.TryParse(col, out int result))
                    {
                        if(result > maxResult)
                        {
                            maxResult = result;
                            bestLocationX = colIndex;
                            bestLocationY = rowIndex;
                        }
                    }

                    colIndex++;
                }

                rowIndex++;
            }

            return (maxResult, bestLocationX, bestLocationY);
        }

        private static List<(int,int)> CalculateAngles((int,int) location, List<(int, int)> asteroidLocations)
        {
            var angles = asteroidLocations
                .Where(x => x != location)
                .Select(x => (x.Item1 - location.Item1, x.Item2 - location.Item2))
                .ToList();

            var returnAngles = new List<(int, int)>();

            foreach(var angle in angles)
            {
                var commonDivisor = gcf(angle.Item1, angle.Item2);
                returnAngles.Add(
                    (commonDivisor > 0 ? angle.Item1 / commonDivisor : -(angle.Item1 / commonDivisor),
                     commonDivisor > 0 ? angle.Item2 / commonDivisor : -(angle.Item2 / commonDivisor))
                    );
            }

            return returnAngles.Distinct().ToList();
        }

        private static int gcf(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        private static void CalculateNearbyAsteroidsFromLocations(List<(int, int)> asteroidLocations, List<List<string>> space)
        {
            foreach(var location in asteroidLocations)
            {
                var foundAsteroids = CalculateAngles(location, asteroidLocations);
                space[location.Item2][location.Item1] = foundAsteroids.Count().ToString();
                
                PrintSpace(space);
            }
        }

        private static List<(int,int)> GetLocationOfAsteroids(List<List<string>> space)
        {
            var rowindex = 0;
            var returnList = new List<(int, int)>();
            foreach(var row in space)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    if (row[i] == "#")
                    {
                        returnList.Add((i, rowindex));
                    }
                }

                rowindex++;
            }

            return returnList;
        }

        public static void PrintSpace(List<List<string>> space)
        {
            Console.WriteLine();
            foreach (var row in space)
            {
                Console.WriteLine(string.Join("", row));
            }
        }
    }
}
