using PuzzleInputParser;
using System;
using System.Collections.Generic;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var intValues = FileReader.GetValues("./input.txt", ",");

            //part 1
            Console.WriteLine(runProgram(intValues, 12, 2));

            for (int noun = 0; noun < 99; noun++)
            {
                for (int verb = 0; verb < 99; verb++)
                {
                    var result = runProgram(FileReader.GetValues("./input.txt", ","), noun, verb);
                    Console.WriteLine(result);

                    if(result == 19690720)
                    {
                        Console.WriteLine($"noun:{noun} verb: {verb}");
                        Console.ReadLine();
                    }
                }
            }
        }

        static int runProgram(List<int> intValues, int noun, int verb)
        {
            int currentPosition = 0;
            var exit = false;

            intValues[1] = noun;
            intValues[2] = verb;

            while (!exit)
            {
                var opcode = intValues[currentPosition];
                if (opcode == 1)
                {
                    var result =
                        intValues[intValues[currentPosition + 1]] +
                        intValues[intValues[currentPosition + 2]];

                    intValues[intValues[currentPosition + 3]] = result;

                }
                else if (opcode == 2)
                {
                    var result =
                        intValues[intValues[currentPosition + 1]] *
                        intValues[intValues[currentPosition + 2]];

                    intValues[intValues[currentPosition + 3]] = result;

                }
                else if (opcode == 99)
                {
                    exit = true;
                }

                currentPosition = currentPosition + 4;
            }

            return intValues[0];
        }
    }
}
