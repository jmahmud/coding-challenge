namespace ConstructionLine.CodingChallenge.Indexing
{
    public class Facet
    {
        public string FacetName { get; set; }
        public int Count { get; set; }

        public Facet(string facetName, int count)
        {
            FacetName = facetName;
            Count = count;
        }
    }
}