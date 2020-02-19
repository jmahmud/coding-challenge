using System;

namespace ConstructionLine.CodingChallenge.Exceptions
{
    public class IndexDocumentAlreadyExists : Exception
    {
        public IndexDocumentAlreadyExists(Guid id) : base($"IndexDocument with id {id} already exisst.")
        {
            
        }
    }
}