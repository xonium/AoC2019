using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2019
{
    class Program
    {
        static void Main(string[] args)
        {
            var masses = FileReader.GetValues("./input_part1.txt", "\r\n");
            var totalFuel = new List<int>();

            foreach(var mass in masses)
            {
                var fuel = Convert.ToInt32(Math.Floor(mass / 3d)) - 2;
                totalFuel.Add(fuel);
            }

            Console.WriteLine($"total fuel part 1: {totalFuel.Sum()}");

            var fuelsFuel = new List<int>();
            foreach (var mass in masses)
            {
                fuelsFuel.Add(AdditionalFuel(mass));
            }

            Console.WriteLine($"total fuel part 2: {fuelsFuel.Sum()}");
        }

        public static int AdditionalFuel(int fuel)
        {
            var additionalFuel = Convert.ToInt32(Math.Floor(fuel / 3d)) - 2;
            if(additionalFuel > 0)
            {
                return additionalFuel + AdditionalFuel(additionalFuel);
            }

            return 0;
        }
        
    }
}
