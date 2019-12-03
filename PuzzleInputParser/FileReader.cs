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

        public static List<List<string>> GetValues(string fileName, string separator, string listseparator)
        {
            var allText = File.ReadAllText(fileName);
            var returnList = new List<List<string>>();
            var lists = allText.Split(listseparator);
            foreach(var list in lists)
            {
                returnList.Add(list.Split(separator).Select(x => x.Trim()).ToList());
            }

            return returnList;
        }
    }
}
