namespace ConstructionLine.CodingChallenge.Indexing
{
    public class Facet
    {
        public string FacetName { get; }
        public int Count { get; }

        public Facet(string facetName, int count)
        {
            FacetName = facetName;
            Count = count;
        }
    }
}