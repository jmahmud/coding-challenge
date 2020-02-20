using System;

namespace ConstructionLine.CodingChallenge.Indexing
{
    public interface IIndexer
    {
        void Index(IndexDocument document);
        IndexDocument GetDocument(Guid id);

        IndexSearchResult Search(IndexSearchOptions searchOptions);
    }
}
