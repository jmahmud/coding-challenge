using System.Collections.Generic;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly List<Shirt> _shirts;

        public SearchEngine(List<Shirt> shirts)
        {
            _shirts = shirts;

            // TODO: data preparation and initialisation of additional data structures to improve performance goes here.


            /*
             * var indexer = new ShirtIndexer()
             * foreach(var s in _shirts)
             * {
             *      indexer.Index(s);
             *      
             * }


            var shirtIndex = new ShirtIndex()
            shirtIndex.Add(shirt)

             *
             * 
             */

        }


        public SearchResults Search(SearchOptions options)
        {
            // TODO: search logic goes here.

            return new SearchResults
            {
            };
        }
    }
}