using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
            var asteroidLocations2 = GetLocationOfAsteroids(space2);
            space2[bestLocation.Item3][bestLocation.Item2] = "X";

            PrintSpace(space2);
            var totalVaporizations = 0;
            while(asteroidLocations2.Count() > 0)
            {
                totalVaporizations = VaporizeAsteroids(space2, (bestLocation.Item2, bestLocation.Item3), asteroidLocations2, totalVaporizations);
                asteroidLocations2 = GetLocationOfAsteroids(space2);
            }
        }

        private static int VaporizeAsteroids(List<List<string>> space, (int, int) baseLocation, List<(int,int)> asteroidLocations, int numbersVaporized = 0)
        {
            var angles2 = asteroidLocations
                .Where(x => x != baseLocation)
                .Select(x => (x.Item1 - baseLocation.Item1, x.Item2 - baseLocation.Item2))
                .ToList();

            var angles = CalculateAngles(baseLocation, asteroidLocations);
            angles.Sort(AnglesSorter);

            foreach(var angle in angles)
            {
                var asteriodsInLine = angles2.Where(location => NormalizedVector(location) == angle).ToList();
                asteriodsInLine.Sort(SortByClosest);
                var location = asteriodsInLine.First();

                numbersVaporized++;
                space[location.Item2 + baseLocation.Item2][location.Item1 + baseLocation.Item1] = ".";
                Console.WriteLine($"Vaporized {numbersVaporized} is at {location.Item1 + baseLocation.Item1},{location.Item2 + baseLocation.Item2}");

                if(numbersVaporized == 200)
                {
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine($"Final answer: {(location.Item1 + baseLocation.Item1) * 100 + (location.Item2 + baseLocation.Item2)}");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                    Console.WriteLine("******************");
                }
            }

            return numbersVaporized;
        }

        private static int SortByClosest((int, int) asteroidOne, (int, int) asteroidTwo)
        {
            var asteroidVectorOne = new Vector2(asteroidOne.Item1, asteroidOne.Item2);
            var asteroidVectorTwo = new Vector2(asteroidTwo.Item1, asteroidTwo.Item2);

            if (asteroidVectorOne.Length() > asteroidVectorTwo.Length())
                return 1;

            return -1;
        }

        private static int AnglesSorter((int, int) angleOne, (int, int) angleTwo)
        {
            if (GetAngle(angleOne) > GetAngle(angleTwo)) return 1;
            
            return -1;
        }

        private static double GetAngle((int, int) angle)
        {
            var radian1 = Math.Atan2(angle.Item2, angle.Item1);
            var degreeResult = radian1 * (180.0 / Math.PI);
            if(degreeResult < 0)
            {
                if(Math.Abs(degreeResult) < 90)
                {
                    return 90 + degreeResult;
                }
                return 360 + degreeResult + 90 == 360 ? 0 : 360 + degreeResult + 90;
            }

            return degreeResult +90;
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
                var vector = NormalizedVector(angle);
                returnAngles.Add((vector.Item1,vector.Item2));
            }

            return returnAngles.Distinct().ToList();
        }

        private static (int,int) NormalizedVector((int,int) angle)
        {
            var commonDivisor = GreatestCommonFactor(angle.Item1, angle.Item2);
            var returnAngleX = commonDivisor > 0 ? angle.Item1 / commonDivisor : -(angle.Item1 / commonDivisor);
            var returnAngleY = commonDivisor > 0 ? angle.Item2 / commonDivisor : -(angle.Item2 / commonDivisor);

            return (returnAngleX, returnAngleY);
        }

        private static int GreatestCommonFactor(int a, int b)
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
