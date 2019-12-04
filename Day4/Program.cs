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
                        Regex regexObj = new Regex(@"([1-9])\1");
                        var doubleValueFound = false;
                        allMatchResults = regexObj.Matches(valueString);
                        if (allMatchResults.Count > 0)
                        {
                            var enumerator = allMatchResults.GetEnumerator();
                            while(enumerator.MoveNext())
                            {
                                var p = (string)enumerator.Current.ToString();
                                var indexOfMatch = valueString.IndexOf(p);

                                if(valueString.Length > indexOfMatch + 2) { 
                                    if(valueString[indexOfMatch] != valueString[indexOfMatch + 2])
                                    {
                                        doubleValueFound = true;
                                        break;
                                    }
                                }
                                else
                                    {
                                    doubleValueFound = true;
                                }
                            }

                            if(doubleValueFound)
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

