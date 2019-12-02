using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PuzzleInputParser
{
    public static class FileReader
    {
        public static List<int> GetValues(string fileName, string separator)
        {
            var allText = File.ReadAllText(fileName);
            return allText.Split(separator).Select(x => int.Parse(x.Trim())).ToList();
        }
    }
}
