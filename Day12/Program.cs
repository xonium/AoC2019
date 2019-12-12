using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var moons = new List<Moon>()
            {
                new Moon(14, 9, 14),
                new Moon(9, 11, 6),
                new Moon(-6, 14, -4),
                new Moon(4, -4, -3)
            };

            CalculateMoonPositionsAndVelocity(moons, 1000, false);

            int totalEnergy = 0;
            foreach (var moon in moons)
            {
                totalEnergy += moon.Energy();
            }

            Console.WriteLine($"Part 1: ----- {totalEnergy} ------");

            var moons2 = new List<Moon>()
            {
                new Moon(-8, -10, 0),
                new Moon(5, 5, 10),
                new Moon(2, -7, 3),
                new Moon(9, -8, -3)
            };

            var moons1 = new List<Moon>()
            {
                new Moon(-1, 0, 2),
                new Moon(2, -10, -7),
                new Moon(4, -8, 8),
                new Moon(3, 5, -1)
            };

            //The axes(x, y, z) are totally independent.So it suffices to find the period for each axis separately.
            //Then the answer is the lcm of these.
            //Each axis will repeat "relatively quickly"(fast enough to brute force)
            //Since each state has a unique parent, the first repeat must be a repeat of state 0.
            var xAxisRepeat = WhenWillXAxisRepeat(moons1);

            moons1 = new List<Moon>()
            {
                new Moon(-1, 0, 2),
                new Moon(2, -10, -7),
                new Moon(4, -8, 8),
                new Moon(3, 5, -1)
            };

            var yAxisRepeat = WhenWillYAxisRepeat(moons1);

            moons1 = new List<Moon>()
            {
                new Moon(-1, 0, 2),
                new Moon(2, -10, -7),
                new Moon(4, -8, 8),
                new Moon(3, 5, -1)
            };

            var zAxisRepeat = WhenWillZAxisRepeat(moons1);

            Console.WriteLine($"Steps x: {xAxisRepeat}, y: {yAxisRepeat}, z: {zAxisRepeat}");
            var result = FindLCM(xAxisRepeat, yAxisRepeat, zAxisRepeat);

            Console.WriteLine($"Part 2: ----- {result} ------");
        }

        private static double FindLCM(long xRepeats, long yRepeats, long zRepeats)
        {
            var factorsX = GetFactors((int)xRepeats);
            var factorsY = GetFactors((int)yRepeats);
            var factorsZ = GetFactors((int)zRepeats);

            var maxX = factorsX.Max();
            var maxY = factorsY.Max();
            var maxZ = factorsZ.Max();

            var countX = factorsX.Where(x => x == maxX).Count();
            var countY = factorsY.Where(x => x == maxY).Count();
            var countZ = factorsZ.Where(x => x == maxZ).Count();

            var result = Math.Pow(maxX, countX) * Math.Pow(maxY, countY) * Math.Pow(maxZ, countZ);

            return result;
        }

        public static IEnumerable<int> Primes()
        {
            var ints = Enumerable.Range(2, Int32.MaxValue - 1);
            return ints.Where(x => !ints
                                    .TakeWhile(y => y < x)
                                    .Any(y => x % y == 0));
        }

        public static IEnumerable<int> GetFactors(int input)
        {
            int first = Primes()
                .TakeWhile(x => x <= Math.Sqrt(input))
                .FirstOrDefault(x => input % x == 0);
            return first == 0
                    ? new[] { input }
                    : new[] { first }.Concat(GetFactors(input / first));
        }

        private static long WhenWillXAxisRepeat(List<Moon> moons)
        {
            long steps = 0;
            var initalStateFound = false;
            while (!initalStateFound)
            {
                ApplyGravity(moons);
                ApplyVelocity(moons);

                foreach (var moon in moons)
                {
                    if(moon.IsAtInitialStateX()) { 
                        initalStateFound = true;
                    }
                    else { 
                        initalStateFound = false;
                        break;
                    }
                }

                steps++;
            }

            return steps;
        }

        private static long WhenWillYAxisRepeat(List<Moon> moons)
        {
            long steps = 0;
            var initalStateFound = false;
            while (!initalStateFound)
            {
                ApplyGravity(moons);
                ApplyVelocity(moons);

                foreach (var moon in moons)
                {
                    if (moon.IsAtInitialStateY())
                    {
                        initalStateFound = true;
                    }
                    else
                    {
                        initalStateFound = false;
                        break;
                    }
                }

                steps++;
            }

            return steps;
        }

        private static long WhenWillZAxisRepeat(List<Moon> moons)
        {
            long steps = 0;
            var initalStateFound = false;
            while (!initalStateFound)
            {
                ApplyGravity(moons);
                ApplyVelocity(moons);

                foreach (var moon in moons)
                {
                    if (moon.IsAtInitialStateZ())
                    {
                        initalStateFound = true;
                    }
                    else
                    {
                        initalStateFound = false;
                        break;
                    }
                }

                steps++;
            }

            return steps;
        }

        private static long NumberOfStepsBeforeInitialState(List<Moon> moons)
        {
            long steps = 0;
            long stepSize = 1;
            PrintMoons(moons, (int)steps);
            var initalStateFound = false;
            while (!initalStateFound)
            {
                ApplyGravity(moons);
                ApplyVelocity(moons);

                foreach (var moon in moons)
                {
                    if(moon.IsAtInitialState()) { 
                        initalStateFound = true;
                        PrintMoons(moons, steps);
                    }
                    else { 
                        initalStateFound = false;
                        break;
                    }
                }

                steps++;
            }

            return steps;
        }

        private static void CalculateMoonPositionsAndVelocity(List<Moon> moons, int steps, bool trace = true)
        {
            var numberOfSteps = steps;
            for (int i = 0; i <= numberOfSteps; i++)
            {
                if(trace) PrintMoons(moons, i);
                if (i == numberOfSteps) break;

                ApplyGravity(moons);
                ApplyVelocity(moons);
            }
        }

        private static void ApplyVelocity(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                moon.Position.X += moon.Velocity.X;
                moon.Position.Y += moon.Velocity.Y;
                moon.Position.Z += moon.Velocity.Z;
            }
        }

        private static void ApplyGravity(List<Moon> moons)
        {
            for (int m = 0; m < moons.Count; m++)
            {
                for (int m2 = 0; m2 < moons.Count; m2++)
                {
                    if (m == m2) continue;

                    if (moons[m].Position.X > moons[m2].Position.X)
                    {
                        moons[m2].Velocity.X += 1;
                        moons[m].Velocity.X += -1;
                    }

                    if (moons[m].Position.Y > moons[m2].Position.Y)
                    {
                        moons[m2].Velocity.Y += 1;
                        moons[m].Velocity.Y += -1;
                    }

                    if (moons[m].Position.Z > moons[m2].Position.Z)
                    {
                        moons[m2].Velocity.Z += 1;
                        moons[m].Velocity.Z += -1;
                    }
                }
            }
        }

        private static void PrintMoons(List<Moon> moons, long i)
        {
            Console.WriteLine($"After {i} steps:");
            foreach (var moon in moons)
            {
                Console.WriteLine(moon.ToString());
            }
        }
    }

    public class Moon
    {
        public Moon(int x, int y, int z)
        {
            Position = new Coordinates() { X = x, Y = y, Z = z };
            Velocity = new Coordinates() { X = 0, Y = 0, Z = 0 };
            InitialPosition = new Coordinates() { X = x, Y = y, Z = z };
            InitialVelocity = new Coordinates() { X = 0, Y = 0, Z = 0 };
        }
        public Coordinates Position { get; set; }

        public Coordinates Velocity { get; set; }

        public Coordinates InitialPosition { get; set; }

        public Coordinates InitialVelocity { get; set; }

        public override string ToString()
        {
            return $"pos=<x={Position.X}, y={Position.Y}, z={Position.Z}>, vel=<x={Velocity.X}, y={Velocity.Y}, z={Velocity.Z}>";
        }

        public bool IsAtInitialState()
        {
            if( Position.X == InitialPosition.X && 
                Position.Y == InitialPosition.Y &&
                Position.Z == InitialPosition.Z &&
                Velocity.X == InitialVelocity.Z &&
                Velocity.Y == InitialVelocity.Y &&
                Velocity.Z == InitialVelocity.Z)
            {
                return true;
            }

            return false;
        }

        public bool IsAtInitialStateX()
        {
            if (Position.X == InitialPosition.X &&
                Velocity.X == InitialVelocity.X)
            {
                return true;
            }

            return false;
        }

        public bool IsAtInitialStateY()
        {
            if (Position.Y == InitialPosition.Y &&
                Velocity.Y == InitialVelocity.Y)
            {
                return true;
            }

            return false;
        }

        public bool IsAtInitialStateZ()
        {
            if (Position.Z == InitialPosition.Z &&
                Velocity.Z == InitialVelocity.Z)
            {
                return true;
            }

            return false;
        }

        public int Energy()
        {
            var pot = Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
            var kin = Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);

            return pot * kin;
        }
    }

    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
