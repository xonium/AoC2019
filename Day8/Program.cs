using PuzzleInputParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            var imageData = FileReader.GetValues("./input.txt");
            var image = GetLayers(imageData, 25, 6);

            var layerIndex = GetLayerWithFewestZerosAndCount(image);
            Console.WriteLine(layerIndex);

            var countedValue = CountValuesOfLayer(layerIndex.Item1, image);

            Console.WriteLine($"--- Counted Value of layer --- {countedValue}");

            var decodedImage = DecodeImage(image);

            PrintDecodedImage(decodedImage);
        }

        private static void PrintDecodedImage(List<List<int>> decodedImage)
        {
            foreach(var row in decodedImage)
            {
                Console.WriteLine(string.Join(string.Empty, row.Select((x) => x == 0 ? " " : "X")));
            }
        }

        private static List<List<int>> DecodeImage(List<List<List<int>>> imageLayers)
        {
            var decodedImage = new List<List<int>>();

            var rowIndex = 0;
            foreach (var rows in imageLayers[0])
            {
                decodedImage.Add(new List<int>());

                var columnIndex = 0;
                foreach (var column in rows)
                {
                    if(column == 2)
                    {
                        foreach(var layer in imageLayers)
                        {
                            if(layer[rowIndex][columnIndex] != 2)
                            {
                                decodedImage[rowIndex].Add(layer[rowIndex][columnIndex]);
                                break;
                            }
                        }                        
                    }
                    else
                    {
                        decodedImage[rowIndex].Add(column);
                    }

                    columnIndex++;
                }

                rowIndex++;
            }

            return decodedImage;
        }

        private static int CountValuesOfLayer(int layerIndex, List<List<List<int>>> image)
        {
            var listOfValues = new List<int>();
            foreach(var row in image[layerIndex])
            {
                foreach(var column in row)
                {
                    if(column == 1 || column == 2)
                    {
                        listOfValues.Add(column);
                    }
                }
            }

            var ones = listOfValues.Where((x) => x == 1);
            var twos = listOfValues.Where((x) => x == 2);

            return ones.Count() * twos.Count();
        }

        private static (int, int) GetLayerWithFewestZerosAndCount(List<List<List<int>>> image)
        {            
            var layerIndex = 0;
            var minTotalZeros = int.MaxValue;
            var minLayerWithMinTotalZeros = 0;

            foreach (var layer in image)
            {
                var totalZeros = 0;

                foreach (var row in layer)
                {
                    foreach (var column in row)
                    {
                        if(column == 0)
                        {
                            totalZeros++;
                        }
                    }
                }

                if(totalZeros < minTotalZeros)
                {
                    minTotalZeros = totalZeros;
                    minLayerWithMinTotalZeros = layerIndex;
                }

                Console.WriteLine((layerIndex, totalZeros));

                layerIndex++;
            }

            return (minLayerWithMinTotalZeros, minTotalZeros);
        }

        private static List<List<List<int>>> GetLayers(List<int> imageData, int wide, int tall)
        {
            var counter = 0;
            var image = new List<List<List<int>>>();

            while(counter < imageData.Count) { 
                var layer = new List<List<int>>(tall);
                for (int i = 0; i < tall; i++)
                {
                    layer.Add(new List<int>());

                    for (int p = 0; p < wide; p++)
                    {
                        if (counter >= imageData.Count) break;

                        layer[i].Add(imageData[counter]);
                        counter++;
                    }
                }

                image.Add(layer);
            }

            return image;
        }
    }
}
