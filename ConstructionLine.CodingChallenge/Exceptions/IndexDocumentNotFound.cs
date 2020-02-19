using System;

namespace ConstructionLine.CodingChallenge.Exceptions
{
    public class IndexDocumentNotFound : Exception
    {
        public IndexDocumentNotFound(Guid missingId) : base($"IndexDocument with is {missingId} not found.")
        {
            
        }
    }
}