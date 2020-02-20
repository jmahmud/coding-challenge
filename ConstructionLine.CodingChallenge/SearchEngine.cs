using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConstructionLine.CodingChallenge.Indexing;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<Guid, Shirt> _shirts;
        private readonly IIndexer _indexer;

        public SearchEngine(List<Shirt> shirts)
        {
            //We shall keep a dictionary of shirts (so we can look up shirts by ID)
            _shirts = shirts.ToDictionary(s => s.Id, s => s);

            // TODO: data preparation and initialisation of additional data structures to improve performance goes here.
            _indexer = new Indexer();
            
            //Convert shirts to documents and index
            _shirts.Values.ToList().ForEach(shirt =>
            {
                _indexer.Index(ToIndexDocument(shirt));
            });

        }


        public SearchResults Search(SearchOptions options)
        {
            // TODO: search logic goes here.
            if(options == null || options.Colors == null || options.Sizes == null)
                throw new ArgumentNullException("Please ensure the SearchOptions is not null, and the the Colors and Size options are not null");
            
            // Convert SearchOptions to IndexSearchOptions
            var indexSearchOptions = ToIndexSearchOptions(options);
            
            //Perform search
            var results = _indexer.Search(indexSearchOptions);
            
            //Convert and results
            return new SearchResults
            {
                Shirts = results.DocumentResults.Select(doc => _shirts[doc.Id]).ToList(),
                ColorCounts = ConvertFacetsToColorCount(results.FacetResults[MakePluralSingular(nameof(options.Colors))]),
                SizeCounts = ConvertFacetsToSizeCount(results.FacetResults[MakePluralSingular(nameof(options.Sizes))])
            };
        }

        private List<ColorCount> ConvertFacetsToColorCount(List<Facet> resultsFacetResult)
        {
            return Color.All.Select(color =>
            {
                var facet = resultsFacetResult.FirstOrDefault(x => x.FacetName == color.Id.ToString());
                
                return new ColorCount()
                {
                    Color = color,
                    Count = facet?.Count ?? 0
                };
            }).ToList();
        }
        
        private List<SizeCount> ConvertFacetsToSizeCount(List<Facet> resultsFacetResult)
        {
            return Size.All.Select(size =>
            { 
               var facet = resultsFacetResult.FirstOrDefault(x => x.FacetName == size.Id.ToString());
                
                return new SizeCount()
                {
                    Size = size,
                    Count = facet?.Count ?? 0
                };
            }).ToList();
        }

        private IndexDocument ToIndexDocument(Shirt shirt)
        {
            return new IndexDocument()
            {
                Id = shirt.Id,
                Fields =
                {
                    { nameof(shirt.Color), shirt.Color?.Id.ToString() },
                    { nameof(shirt.Size), shirt.Size?.Id.ToString() }
                }
            };
        }

        private IndexSearchOptions ToIndexSearchOptions(SearchOptions searchOptions)
        {
            var indexSearchOptions = new IndexSearchOptions();
            
            indexSearchOptions.SearchBy(MakePluralSingular(nameof(searchOptions.Colors)), searchOptions.Colors.Select(c => c.Id.ToString()).ToList());
            indexSearchOptions.SearchBy(MakePluralSingular(nameof(searchOptions.Sizes)), searchOptions.Sizes.Select(c => c.Id.ToString()).ToList());
            
            return indexSearchOptions;
        }

        private string MakePluralSingular(string word)
        {
            if (word.EndsWith('s'))
            {
                return word.Substring(0, word.Length - 1);
            }

            return word;
        }
    }
}