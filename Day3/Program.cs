using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day3
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}";
        }

        public override bool Equals(object obj)
        {
            return ((Position)obj).X == this.X && ((Position)obj).Y == this.Y;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var wires = FileReader.GetValues("./input.txt", ",", "\r\n");
            var generatedWires = new List<List<Position>>();

            foreach (var wire in wires)
            {
                generatedWires.Add(generateWire(wire));
            }

            var minimumLength = int.MaxValue;
            // wire 0
            foreach (var position in generatedWires[0])
            {
                var p = generatedWires[1].Find(x => x.Equals(position));
                if (p != null)
                {
                    if ((Math.Abs(position.X) + Math.Abs(position.Y)) < minimumLength)
                    {
                        minimumLength = Math.Abs(position.X) + Math.Abs(position.Y);
                        Console.WriteLine(Math.Abs(position.X) + Math.Abs(position.Y));
                    }
                }
            }

            Console.WriteLine("bugg?!");
            Console.ReadLine();
        }

        private static List<Position> generateWire(List<string> wire)
        {
            var positions = new List<Position>();
            var currentPosition = new Position { X = 0, Y = 0 };

            foreach (var length in wire)
            {
                var lengthEnum = length.Skip(1);
                var lengthOfWire = int.Parse(string.Join("", lengthEnum));
                var direction = length[0];

                switch (direction)
                {
                    case 'R':
                        for (int i = 0; i < lengthOfWire; i++)
                        {
                            currentPosition.X += 1;
                            positions.Add(new Position { X = currentPosition.X, Y = currentPosition.Y });
                        }
                        break;
                    case 'L':
                        for (int i = 0; i < lengthOfWire; i++)
                        {
                            currentPosition.X -= 1;
                            positions.Add(new Position { X = currentPosition.X, Y = currentPosition.Y });
                        }
                        break;
                    case 'U':
                        for (int i = 0; i < lengthOfWire; i++)
                        {
                            currentPosition.Y += 1;
                            positions.Add(new Position { X = currentPosition.X, Y = currentPosition.Y });
                        }
                        break;
                    case 'D':
                        for (int i = 0; i < lengthOfWire; i++)
                        {
                            currentPosition.Y -= 1;
                            positions.Add(new Position { X = currentPosition.X, Y = currentPosition.Y });
                        }
                        break;                        
                }

            }

            return positions;
        }
    }
}
