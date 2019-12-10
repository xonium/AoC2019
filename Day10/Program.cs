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
            var space = FileReader.GetValuesList("./input.txt", "\r\n");
            
            PrintSpace(space);
            var asteroidLocations = GetLocationOfAsteroids(space);

            CalculateNearbyAsteroidsFromLocations(asteroidLocations, space);
            var numberOfAsteroidsFound = GetBestLocation(space);
            Console.WriteLine($"-- Best location spots: {numberOfAsteroidsFound} asteroids --");
        }

        private static int GetBestLocation(List<List<string>> space)
        {
            var maxResult = 0;
            foreach(var row in space)
            {
                foreach(var col in row)
                {
                    if(int.TryParse(col, out int result))
                    {
                        if(result > maxResult)
                        {
                            maxResult = result;
                        }
                    }
                }
            }

            return maxResult;
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

        static int gcf(int a, int b)
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
