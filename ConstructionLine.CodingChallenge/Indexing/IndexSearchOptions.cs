using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge.Indexing
{
    public class IndexSearchOptions
    {
        public Dictionary<string, List<string>> SearchOptions { get; set; }

        public IndexSearchOptions()
        {
            SearchOptions = new Dictionary<string, List<string>>();
        }

        public void SearchBy(string fieldName, List<string> fieldValues)
        {
            SearchOptions.Add(fieldName, fieldValues);
        }
    }
}