using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        public static int Count { get; set; }
        static void Main(string[] args)
        {
            var orbits = FileReader.GetValuesString("./input.txt", "\r\n");
            var orbitChart = FindOrbits("COM", orbits, 0);
            CountOrbits(orbitChart);
            Console.WriteLine($"-- TotalOrbits --: {Count}");

        }

        private static void CountOrbits(OrbitChart orbitChart)
        {
            if (orbitChart.InOrbit == null) {
                Count = Count + orbitChart.Depth;
                return;
            }
            foreach (var inOrbit in orbitChart.InOrbit)
            {
                CountOrbits(inOrbit);
            }

            Count = Count + orbitChart.Depth;
        }

        private static OrbitChart FindOrbits(string PlanetToFind, List<string> orbits, int depth)
        {
            var planetWithOrbits = orbits.Where(x => x.Contains(PlanetToFind + ")"));
            var orbitChart = new OrbitChart();
            orbitChart.Depth = depth;
            Console.WriteLine($"depth: {depth}");

            foreach (var orbit in planetWithOrbits)
            {
                if (orbitChart.PlanetName == null) {
                    orbitChart = new OrbitChart(orbit);
                    orbitChart.Depth = depth;
                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits, depth + 1);
                    if(foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            foundOrbits
                        };
                    }
                    else
                    {
                        var chart = new OrbitChart(orbit.Split(')')[1]);
                        chart.Depth = depth + 1;
                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            chart
                        };
                    }
                }
                else
                {
                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits, depth + 1);
                    if (foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit.Add(foundOrbits);
                    }
                    else
                    {
                        var chart = new OrbitChart(orbit.Split(')')[1]);
                        chart.Depth = depth + 1;
                        orbitChart.InOrbit.Add(chart);
                    }
                        
                }
            }

            return orbitChart;
        }

        internal class OrbitChart
        {
            public OrbitChart() { }
            public OrbitChart(string input)
            {
                var splitted = input.Split(')');
                if(splitted.Length == 2) { 
                    PlanetName = splitted[0];
                }
                else
                {
                    PlanetName = input;
                    InOrbit = null;
                }
            }

            public string PlanetName { get; set; }

            public int Depth { get; set; }

            public List<OrbitChart> InOrbit { get; set; }

            public override string ToString()
            {
                return PlanetName;
            }
        }
    }
}
