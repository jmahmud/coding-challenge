using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using ConstructionLine.CodingChallenge.Exceptions;
using NUnit.Framework;
using ConstructionLine.CodingChallenge.Indexing;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class IndexerTests
    {
        private IIndexer _sut;
        private Fixture _fixture;
        private Random _random;

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
            _sut = new Indexer();
            _random = new Random();
        }

        #region GetDocumentById
        [Test]
        public void GetDocumentById()
        {
            //Arrange
            var document = _fixture.Create<IndexDocument>();
            
            //Act
            _sut.Index(document);
            
            //Assert
            var retrievedDocument = _sut.GetDocument(document.Id);
            Assert.AreEqual(document, retrievedDocument);
        }
        
        [Test]
        public void GivenDocumentDoesNotExistInIndex_ThenThrowIndexDocumentNotFoundException()
        {
            //Arrange
            var missingId = Guid.NewGuid();
           
            //Assert
            Assert.Throws<IndexDocumentNotFound>(() =>_sut.GetDocument(missingId));
        }

        [Test]
        public void IndexDocument_GivenDocumentAlreadyExists_ThrowsIndexDocumentAlreadyExistsException()
        {
            //Arrange
            var document = _fixture.Create<IndexDocument>();
            
            //Act
            _sut.Index(document);
            Assert.Throws<IndexDocumentAlreadyExists>(() =>_sut.Index(document));
        }
        #endregion
        
        #region Index and Search

        [Test]
        public void Search_WhenSearchingByANonExistentField_ReturnsNoResults()
        {
            //Arrange
            var document = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Large"}
                }
            };
            
            var document2 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Medium"}
                }
            };
            
            var document3 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Blue"},
                    {"Size", "Small"}
                }
            };
            
            _sut.Index(document);
            _sut.Index(document2);
            _sut.Index(document3);

            var searchOptions = new IndexSearchOptions();
            var fieldName = Guid.NewGuid().ToString();
            searchOptions.SearchBy(fieldName, new List<string>() { "Small", "Medium"});
            
            //Act
            var result = _sut.Search(searchOptions);
            
            //Assert
            var documentResults = result.DocumentResults;
            
            Assert.IsNotNull(documentResults);
            Assert.AreEqual(0, documentResults.Count());

        }
        
        
        [Test]
        public void Search_MultipleValuesForSingleField()
        {
            //Arrange
            var document = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Large"}
                }
            };
            
            var document2 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Medium"}
                }
            };
            
            var document3 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Blue"},
                    {"Size", "Small"}
                }
            };
            
            _sut.Index(document);
            _sut.Index(document2);
            _sut.Index(document3);

            var searchOptions = new IndexSearchOptions();
            searchOptions.SearchBy("Size", new List<string>() { "Small", "Medium"});
            
            //Act
            var result = _sut.Search(searchOptions);
            
            //Assert
            var documentResults = result.DocumentResults;
            
            Assert.IsNotNull(documentResults);
            Assert.AreEqual(2, documentResults.Count());
            
            Assert.AreEqual(document2, documentResults.First(x => x.Id == document2.Id));
            Assert.AreEqual(document3, documentResults.First(x => x.Id == document3.Id));            
        }

        [Test]
        public void Search_SingleValueForMultipleFields()
        {
            //Arrange
            var document = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Large"}
                }
            };
            
            var document2 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Medium"}
                }
            };
            
            var document3 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Blue"},
                    {"Size", "Small"}
                }
            };
            
            _sut.Index(document);
            _sut.Index(document2);
            _sut.Index(document3);
            
            var searchOptions = new IndexSearchOptions();
            searchOptions.SearchBy("Size", new List<string>() { "Small"});
            searchOptions.SearchBy("Color", new List<string>() { "Blue"});
            
            //Act
            var result =  _sut.Search(searchOptions);
            
            //Assert
            var documentResults = result.DocumentResults;
            
            Assert.IsNotNull(documentResults);
            Assert.AreEqual(1, documentResults.Count());
            Assert.AreEqual(document3, documentResults.First(x => x.Id == document3.Id));
            
            var facetResults = result.FacetResults;
            Assert.IsNotNull(facetResults);
            Assert.AreEqual(1, facetResults["Size"].First(f => f.FacetName == "Small").Count);
            Assert.AreEqual(1, facetResults["Color"].First(f => f.FacetName == "Blue").Count);
         
        }

        [Test]
        public void Search_MulitpleValuesForMultipleFields()
        {
            //Arrange
            var document = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Large"}
                }
            };
            
            var document2 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Red"},
                    {"Size", "Medium"}
                }
            };
            
            var document3 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Blue"},
                    {"Size", "Small"}
                }
            };
            
            var document4 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Blue"},
                    {"Size", "Medium"}
                }
            };
            
            var document5 = new IndexDocument()
            {
                Id = Guid.NewGuid(),
                Fields =
                {
                    {"Color", "Green"},
                    {"Size", "Large"}
                }
            };
            
            _sut.Index(document);
            _sut.Index(document2);
            _sut.Index(document3);
            _sut.Index(document4);
            _sut.Index(document5);
            
            var searchOptions = new IndexSearchOptions();
            searchOptions.SearchBy("Size", new List<string>() { "Small", "Medium"});
            searchOptions.SearchBy("Color", new List<string>() { "Red", "Blue"});
            
            //Act
            var result =  _sut.Search(searchOptions);
            
            //Assert
            var documentResults = result.DocumentResults;
            
            Assert.IsNotNull(documentResults);
            Assert.AreEqual(3, documentResults.Count());
            Assert.AreEqual(document2, documentResults.First(x => x.Id == document2.Id));
            Assert.AreEqual(document3, documentResults.First(x => x.Id == document3.Id));
            Assert.AreEqual(document4, documentResults.First(x => x.Id == document4.Id));

            var facetResults = result.FacetResults;
            Assert.IsNotNull(facetResults);
            Assert.AreEqual(1, facetResults["Size"].First(f => f.FacetName == "Small").Count);
            Assert.AreEqual(2, facetResults["Size"].First(f => f.FacetName == "Medium").Count);
            
            Assert.AreEqual(1, facetResults["Color"].First(f => f.FacetName == "Red").Count);
            Assert.AreEqual(2, facetResults["Color"].First(f => f.FacetName == "Blue").Count);
        }

        [Test]
        public void IndexerPerformanceTest()
        {
            var documents = Enumerable.Range(0, 50000)
                .Select(i => new IndexDocument(){ Id = Guid.NewGuid(), Fields =
                {
                    {"Size", GetRandomSize()},
                    {"Color", GetRandomColor()},
                }})
                .ToList();
            
            documents.ForEach(d =>
            {
                _sut.Index(d);
            });
            
            var sw = new Stopwatch();
            sw.Start();
            var searchOptions = new IndexSearchOptions();
            searchOptions.SearchBy("Color", new List<string>() { "Red" });

            var results =  _sut.Search(searchOptions);
            sw.Stop();
            Assert.True(sw.ElapsedMilliseconds < 100);
            Console.WriteLine($"Total Milliseconds: {sw.ElapsedMilliseconds}"); 
            
            var documentResults = results.DocumentResults;
            var count = documentResults.Count();
            var facetCount = results.FacetResults["Color"].First().Count;
            Console.WriteLine($"Document Count: {count}");             
            Console.WriteLine($"Facet Count: {facetCount}");             
        }

        private string GetRandomColor()
        {
            return Color.All[_random.Next(0, Color.All.Count - 1)].Name.ToString();
        }
        
        private string GetRandomSize()
        {
            return Size.All[_random.Next(0, Size.All.Count - 1)].Name.ToString();
        }
        
        #endregion
        
        
    }
}
