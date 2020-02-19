using System;
using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge.Indexing
{
    public class IndexDocument
    {
        /// <summary>
        /// To uniquely identify the document
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Fields to index and search by
        /// </summary>
        public Dictionary<string, string> Fields { get; private set; }

        public IndexDocument()
        {
            Fields = new Dictionary<string, string>();
        }


    }
}
