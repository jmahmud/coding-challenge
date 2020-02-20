using System;
using System.Collections.Generic;
using System.Linq;
using ConstructionLine.CodingChallenge.Exceptions;

namespace ConstructionLine.CodingChallenge.Indexing
{
    /// <summary>
    /// Class which holds in memory the documents that have been indexed and a set of inverted indexes for for fields in the document
    /// </summary>
    public class Indexer : IIndexer
    {
        private readonly Dictionary<Guid, IndexDocument> _documents;
        private readonly Dictionary<string, Dictionary<string, List<Guid>>> _invertedIndexes;

        public Indexer()
        {
            _documents = new Dictionary<Guid, IndexDocument>();
            _invertedIndexes = new Dictionary<string, Dictionary<string, List<Guid>>>();
        }

        public void Index(IndexDocument document)
        {
            //We shall keep a dictionary of the original document
            if (!_documents.TryAdd(document.Id, document))
            {
                throw new IndexDocumentAlreadyExists(document.Id);
            }
            
            //create inverted indexes for each field
            document.Fields.Keys.ToList().ForEach(fieldName =>
            {
                //get or create inverted index for fieldname
                var invertedIndexForField = GetOrCreateInvertedIndex(fieldName);
                
                //if inverted index does not have field value, add it
                var fieldValue = document.Fields[fieldName];
                if (fieldValue != null)
                {
                    var documentIdListForInvertedIndex = GetOrCreateDocumentIdListForInvertedIndex(invertedIndexForField, fieldValue);

                    //add document Id for field value - storing just the ID and not the document, and performance is better with value type
                    documentIdListForInvertedIndex.Add(document.Id);    
                }
            });
        }

        private List<Guid> GetOrCreateDocumentIdListForInvertedIndex(Dictionary<string, List<Guid>> invertedIndexForField, string fieldValue)
        {
            List<Guid> documentIds;
            if (!invertedIndexForField.TryGetValue(fieldValue, out documentIds))
            {
                documentIds = new List<Guid>();
                invertedIndexForField.Add(fieldValue,documentIds);
            }
            return documentIds;
        }

        private Dictionary<string, List<Guid>> GetOrCreateInvertedIndex(string fieldName)
        {
            Dictionary<string, List<Guid>> invertedIndex;
            if (!_invertedIndexes.TryGetValue(fieldName, out invertedIndex))
            {
                invertedIndex = new Dictionary<string, List<Guid>>();
                _invertedIndexes.Add(fieldName, invertedIndex);
            }

            return invertedIndex;
        }

        public IndexSearchResult Search(IndexSearchOptions searchOptions)
        {
            var result = new IndexSearchResult();
            
            //Get Search Result documents
            var resultDocumentIds = new List<Guid>();
            searchOptions.SearchOptions.Keys.ToList().ForEach(fieldName =>
            {
                var fieldValues = searchOptions.SearchOptions[fieldName];
                //Only perform a search if there are values to search for
                if (fieldValues.Any())
                {
                    var documentIdsFoundForField = GetDocumentIdsForFieldAndValues(fieldName, fieldValues);
                
                    //we must now to an intersection with the current set of results across other fields
                    if (resultDocumentIds.Count == 0)
                    {
                        //If there are no final results, we'll just add the current set
                        resultDocumentIds.AddRange(documentIdsFoundForField);
                    }
                    resultDocumentIds = resultDocumentIds.Intersect(documentIdsFoundForField).ToList();    
                }
            });

            //Convert the result ids to the documents
            result.DocumentResults = resultDocumentIds.Select(id => _documents[id]);;

            //Calculate facets using the search results documentIds
            _invertedIndexes.Keys.ToList().ForEach(facetFieldName =>
            {
                var facets = new List<Facet>();
                var facetFieldValues = _invertedIndexes[facetFieldName].Keys.ToList();

                if (facetFieldValues.Any())
                {
                    facetFieldValues.ForEach(facetFieldValue =>
                    {
                        var documentIdsForFieldAndValue = GetDocumentIdsForFieldAndValue(facetFieldName, facetFieldValue);
                        if (documentIdsForFieldAndValue != null)
                        {
                            var facetCount = resultDocumentIds.Intersect(documentIdsForFieldAndValue).Count();
                            facets.Add(new Facet(facetFieldValue, facetCount));                        
                        }
                    });
                }
                result.FacetResults.Add(facetFieldName, facets);    
                
            });
            
            return result;

        }

        private List<Guid> GetDocumentIdsForFieldAndValues(string fieldName, List<string> fieldValues)
        {
            var documentIdsFoundForField = new List<Guid>();
            //Go through each field value and get the total document Ids
            fieldValues.ForEach(fieldValue =>
            {
                var documentIdsFoundForFieldAndValue = GetDocumentIdsForFieldAndValue(fieldName, fieldValue);
                if(documentIdsFoundForFieldAndValue != null)
                    documentIdsFoundForField.AddRange(documentIdsFoundForFieldAndValue);
            });

            //Do a distinct (as we don't want duplicates
            return documentIdsFoundForField.Distinct().ToList();
        }
        private List<Guid> GetDocumentIdsForFieldAndValue(string fieldName, string fieldValue)
        {
            if (_invertedIndexes.ContainsKey(fieldName) && _invertedIndexes[fieldName].ContainsKey(fieldValue))
            {
                return _invertedIndexes[fieldName][fieldValue];                
            }

            return null;
        }
        
        public IndexDocument GetDocument(Guid id)
        {
            if (!_documents.ContainsKey(id))
            {
                throw new IndexDocumentNotFound(id);
            }
            return _documents[id];
        }
    }
}
