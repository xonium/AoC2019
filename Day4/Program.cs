using PuzzleInputParser;
using System;
using System.Text.RegularExpressions;

namespace Day4
{
    class Program
    {
        static void Main(string[] args)
        {
            var values = FileReader.GetValues("./input.txt", "-");
            var numberOfPasswords = 0;
            for (int i = values[0]; i <= values[1]; i++)
            {
                var valueString = i.ToString();
                var span = valueString.AsSpan();
                var neverDecreasingInt = 0;
                var isIncreasing = false;
                foreach (var intValue in span)
                {
                    var parsed = int.Parse(intValue.ToString());
                    if(parsed >= neverDecreasingInt)
                    {
                        //ok
                        neverDecreasingInt = parsed;
                        isIncreasing = true;
                    }
                    else
                    {
                        isIncreasing = false;
                        break;
                    }
                }

                if(isIncreasing)
                {

                    MatchCollection allMatchResults = null;
                    try
                    {
                        var doublez = false;
                        Regex regexObj = new Regex(@"([1-9])\1");
                        allMatchResults = regexObj.Matches(valueString);
                        if (allMatchResults.Count > 0)
                        {
                            // Access individual matches using allMatchResults.Item[]
                            var enumerator = allMatchResults.GetEnumerator();
                            while(enumerator.MoveNext())
                            {
                                if(doublez)
                                {
                                    continue;
                                }

                                var p = (string)enumerator.Current.ToString();
                                var indexOfMatch = valueString.IndexOf(p);

                                if(valueString.Length > indexOfMatch + 2) { 
                                    if(valueString[indexOfMatch] != valueString[indexOfMatch + 2])
                                    {                                            
                                        doublez = false;
                                    }
                                    else
                                    {
                                        doublez = true;
                                    }
                                }
                                else
                                    {
                                    doublez = false;
                                }
                            }

                            if(!doublez)
                            {
                                numberOfPasswords++;
                                Console.WriteLine(valueString);
                                Console.WriteLine(numberOfPasswords);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        // Syntax error in the regular expression
                    }

                }
            }

            Console.WriteLine("DONE");
        }
    }
}

