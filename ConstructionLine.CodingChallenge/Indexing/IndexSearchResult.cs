using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge.Indexing
{
    public class IndexSearchResult
    {
        public IEnumerable<IndexDocument>  DocumentResults { get; set; }
        public Dictionary<string, List<Facet>> FacetResults { get; set; }

        public IndexSearchResult()
        {
            DocumentResults = new List<IndexDocument>();
            FacetResults = new Dictionary<string, List<Facet>>();
        }
    }
}