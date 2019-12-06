using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            var orbits = FileReader.GetValuesString("./input.txt", "\r\n");
            var orbitChart = FindOrbits("COM", orbits);


        }

        private static OrbitChart FindOrbits(string PlanetToFind, List<string> orbits)
        {
            var planetWithOrbits = orbits.Where(x => x.Contains(PlanetToFind + ")"));
            var orbitChart = new OrbitChart();
            foreach (var orbit in planetWithOrbits)
            {
                if (orbitChart.PlanetName == null) {
                    orbitChart = new OrbitChart(orbit);

                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits);
                    if(foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            FindOrbits(orbit.Split(')')[1], orbits)
                        };
                    }
                    else
                    {
                        orbitChart.InOrbit = new List<OrbitChart>
                        {
                            new OrbitChart(orbit.Split(')')[1])
                        };
                    }
                }
                else
                {
                    var foundOrbits = FindOrbits(orbit.Split(')')[1], orbits);
                    if (foundOrbits.PlanetName != null)
                    {
                        orbitChart.InOrbit.Add(FindOrbits(orbit.Split(')')[1], orbits));
                    }
                    else
                    {
                        orbitChart.InOrbit.Add(new OrbitChart(orbit.Split(')')[1]));
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

            public List<OrbitChart> InOrbit { get; set; }

            public override string ToString()
            {
                return PlanetName;
            }
        }
    }
}
