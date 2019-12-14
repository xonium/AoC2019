using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day14
{
    class Program
    {
        public static List<Reaction> Reactions { get; set; }

        public static int TotalOres { get; set; }

        static void Main(string[] args)
        {
            var formulas = FileReader.GetValuesString("./input.txt", "\r\n");

            Reactions = new List<Reaction>();
            foreach(var formula in formulas)
            {
                var usesProcudesList = formula.Split("=>").ToList();

                var usesMaterialsString = usesProcudesList[0];
                var producesMaterialsString = usesProcudesList[1];

                var materialUnitList = usesMaterialsString.Split(",").ToList();

                var materials = new List<Material>();
                foreach(var materialUnit in materialUnitList)
                {
                    materials.Add(new Material(materialUnit));
                }

                var producesMaterial = new Material(producesMaterialsString);

                Reactions.Add(new Reaction(materials, producesMaterial));
            }

            Console.WriteLine($"-- Ores: {0} for 1 Fuel --");
        }

        public static void TraverseReactions(string fromType, int unitsRequired)
        {
            var reaction = Reactions.Where(x => x.Produces.Type == fromType).FirstOrDefault();
            var factor = unitsRequired;
            if(reaction.Produces.Units != unitsRequired)
            {
                if(reaction.Produces.Units < unitsRequired)
                {
                    factor = (int)Math.Ceiling(unitsRequired / (float)reaction.Produces.Units);
                }
            }
            else
            {
                reaction.Produces.NormalizedValue = unitsRequired;

                foreach (var requires in reaction.Requires)
                {
                    requires.NormalizedValue = requires.Units * unitsRequired;
                }
            }

            foreach (var reactionRequires in reaction.Requires)
            {
                if(reactionRequires.Type == "ORE")
                {
                    //done
                    TotalOres += reactionRequires.Units * factor;
                    return;
                }
                else
                {
                    TraverseReactions(reactionRequires.Type, factor);
                }
                
            }
        }

        public static void TraverseAndAddOres(string fromType)
        {
            var reaction = Reactions.Where(x => x.Produces.Type == fromType).FirstOrDefault();

            foreach (var reactionRequires in reaction.Requires)
            {
                if (reactionRequires.Type == "ORE")
                {
                    TotalOres = TotalOres + reactionRequires.NormalizedValue;
                    return;
                }
                else
                {
                    TraverseAndAddOres(reactionRequires.Type);
                }

            }
        }
    }

    public class FuelCost
    {
        public FuelCost(int cost, string material)
        {
            Material = new Material(material, cost);
            FuelCosts = new List<FuelCost>();
        }

        public Material Material { get;set;}

        public List<FuelCost> FuelCosts { get; set; }

        public override string ToString()
        {
            return $"{Material.Type} = {Material.Units}, {Material.NormalizedValue}";
        }
    }

    public class Material
    {
        public Material()
        {

        }

        public Material(string material)
        {
            var materialSplitted = material.Trim().Split(" ").ToList();
            Units = int.Parse(materialSplitted[0]);
            Type = materialSplitted[1];
        }

        public Material(string type, int units)
        {
            Type = type;
            Units = units;
        }

        public int NormalizedValue { get; set; }
        public int Units { get; set; }

        public string Type { get; set; }

        public override string ToString()
        {
            return $"{NormalizedValue} {Type}";
        }
    }

    public class Reaction
    {
        public Reaction(List<Material> requires, Material produces)
        {
            Requires = requires;
            Produces = produces;
        }

        public List<Material> Requires { get; set; }

        public Material Produces { get; set; }

        public override string ToString()
        {
            return $"{string.Join(",", Requires)} => {Produces.ToString()}";
        }
    }
}
