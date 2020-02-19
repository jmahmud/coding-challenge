using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ConstructionLine.CodingChallenge.Indexing
{
    public interface IIndexer
    {
        void Index(IndexDocument document);
        IndexDocument GetDocument(Guid id);

        Task<IndexSearchResult> Search(IndexSearchOptions searchOptions);
    }
}
