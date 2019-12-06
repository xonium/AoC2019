using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        public static int Count { get; set; }
        public static OrbitChart YouChart { get; set; }
        public static OrbitChart SanChart { get; set; }
        static void Main(string[] args)
        {
            var orbits = FileReader.GetValuesString("./input.txt", "\r\n");
            var orbitChart = FindOrbits("COM", orbits, 0);
            CountOrbits(orbitChart);
            Console.WriteLine($"-- TotalOrbits --: {Count}");

            var path = new List<string>();
            FindYou(orbitChart, "YOU");
            FindSanta(orbitChart, "SAN");

            var pathToRootYou = PathToRoot(YouChart);
            var pathToRootSan = PathToRoot(SanChart);

            var shortestPath = pathToRootYou.Except(pathToRootSan).Count() + pathToRootSan.Except(pathToRootYou).Count();

            Console.WriteLine($"-- ShortestTravelPath --: {shortestPath}");
        }


        private static List<string> PathToRoot(OrbitChart inputChart)
        {
            var path = new List<string>();
            var chart = inputChart;
            while (chart.Parent != null)
            {
                path.Add(chart.Parent.ToString());
                chart = chart.Parent;
            }

            return path;
        }

        private static void FindYou(OrbitChart orbitChart, string name)
        {
            if (orbitChart.PlanetName == name)
            {
                YouChart = orbitChart;
            }
            else {
                if(orbitChart.InOrbit != null)
                foreach (var inOrbit in orbitChart.InOrbit)
                {
                    FindYou(inOrbit, name);
                }
            }
        }

        private static void FindSanta(OrbitChart orbitChart, string name)
        {
            if (orbitChart.PlanetName == name)
            {
                SanChart = orbitChart;
            }
            else
            {
                if (orbitChart.InOrbit != null)
                foreach (var inOrbit in orbitChart.InOrbit)
                {
                    FindSanta(inOrbit, name);
                }
            }
        }

        private static void CountOrbits(OrbitChart orbitChart)
        {
            if (orbitChart.InOrbit == null) {
                Count += orbitChart.Depth;
                return;
            }
            foreach (var inOrbit in orbitChart.InOrbit)
            {
                CountOrbits(inOrbit);
            }

            Count += orbitChart.Depth;
        }

        private static OrbitChart FindOrbits(string PlanetToFind, List<string> orbits, int depth, OrbitChart parent = null)
        {
            var planetWithOrbits = orbits.Where(x => x.Contains(PlanetToFind + ")"));
            var orbitChart = new OrbitChart
            {
                Depth = depth
            };

            Console.WriteLine($"depth: {depth}");

            foreach (var orbit in planetWithOrbits)
            {
                if (orbitChart.PlanetName == null) {
                    orbitChart = new OrbitChart(planetname: orbit.Split(')')[0])
                    {
                        Depth = depth,
                        Parent = parent
                    };

                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits, depth + 1, orbitChart);
                    if(foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            foundOrbits
                        };
                    }
                    else
                    {
                        var inOrbit = new OrbitChart(planetname: orbit.Split(')')[1])
                        {
                            Depth = depth + 1,
                            Parent = orbitChart
                        };

                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            inOrbit
                        };
                    }
                }
                else
                {
                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits, depth + 1, orbitChart);
                    if (foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit.Add(foundOrbits);
                    }
                    else
                    {
                        var inOrbit = new OrbitChart(planetname: orbit.Split(')')[1])
                        {
                            Depth = depth + 1,
                            Parent = orbitChart
                        };
                        orbitChart.InOrbit.Add(inOrbit);
                    }
                        
                }
            }

            return orbitChart;
        }

        internal class OrbitChart
        {
            public OrbitChart() { }
            public OrbitChart(string planetname)
            {
                PlanetName = planetname;
                InOrbit = null;
            }

            public OrbitChart Parent { get; set; }

            public string PlanetName { get; set; }

            public int Depth { get; set; }

            public List<OrbitChart> InOrbit { get; set; }

            public override string ToString()
            {
                if(Parent == null)
                {
                    return PlanetName;
                }
                else
                {
                    return $"{Parent.PlanetName}){PlanetName}";
                }
                
            }
        }
    }
}
