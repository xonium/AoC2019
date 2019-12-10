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
        }

        private static void CalculateNearbyAsteroidsFromLocations(List<(int, int)> asteroidLocations, List<List<string>> space)
        {
            foreach(var location in asteroidLocations)
            {
                var foundAsteroids = ShootAsteroidDetectionRayVersion2(location, asteroidLocations, space.Count);
                space[location.Item2][location.Item1] = foundAsteroids.ToString();
                
                PrintSpace(space);
            }
        }

        private static int ShootAsteroidDetectionRayVersion2((int, int) location, List<(int, int)> asteroidLocations, int maxSizeOfSpace)
        {
            var numbersFound = 1;
            var angles = new List<(int, int)>
            {
                (1, 0),
                (1, 1),
                (0, 1),
                (2, 1),
                (1, 2),
                (3, 1),
                (3, 2),
                (2, 3),
                (1, 3),
                (4, 1),
                (4, 3),
                (3, 4),
                (1, 4),


                (-1, 0),
                (-1, 1),
                (-0, 1),
                (-2, 1),
                (-1, 2),
                (-3, 1),
                (-3, 2),
                (-2, 3),
                (-1, 3),
                (-4, 1),
                (-4, 3),
                (-3, 4),
                (-1, 4),

                (1, -0),
                (1, -1),
                (0, -1),
                (2, -1),
                (1, -2),
                (3, -1),
                (3, -2),
                (2, -3),
                (1, -3),
                (4, -1),
                (4, -3),
                (3, -4),
                (1, -4),

                (-1, -0),
                (-1, -1),
                (-0, -1),
                (-2, -1),
                (-1, -2),
                (-3, -1),
                (-3, -2),
                (-2, -3),
                (-1, -3),
                (-4, -1),
                (-4, -3),
                (-3, -4),
                (-1, -4),
            };

            foreach(var angle in angles) { 
                var shotRay = 1;
                var currentRayPosition = location;

                while (shotRay < maxSizeOfSpace)
                {
                    currentRayPosition.Item1 += angle.Item1 * shotRay;
                    currentRayPosition.Item2 += angle.Item2 * shotRay;

                    if (asteroidLocations.Contains(currentRayPosition))
                    {
                        numbersFound++;
                        break;
                    }

                    shotRay++;
                }
            }

            return numbersFound;
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
