using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        //Search with single color
        //Search with multiple colours
        
        private static List<List<Color>>  _colorTestsData = new List<List<Color>>()
        {
            new List<Color>() { Color.Red },
            new List<Color>() { Color.Red, Color.Blue },
        };
        
        [Test]
        [TestCaseSource("_colorTestsData")]
        public void WhenSearchingByColors_ThenResultsExpectedResults(List<Color> colors)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = colors,
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }
        
        
        private static List<List<Size>>  _sizeTestsData = new List<List<Size>>()
        {
            new List<Size>() { Size.Small },
            new List<Size>() { Size.Small, Size.Medium },
        };
        
        //Search with single size
        //Search with multiple sizes
        [Test]
        [TestCaseSource("_sizeTestsData")]
        public void WhenSearchingBySizes_ThenResultsExpectedResults(List<Size> sizes)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Sizes = sizes,
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }
        
        //Search with single color and single size (original test)
        [Test]
        public void Test()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red},
                Sizes = new List<Size> {Size.Small}
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }
        
        
        //Search with multiple colors and sizes
        [Test]
        public void WhenSearchingByMultiplerSizesAndMultipleColors_TheExpectedResultsReturn()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
                new Shirt(Guid.NewGuid(), "Black - Small", Size.Small, Color.Black),
                new Shirt(Guid.NewGuid(), "Yello - Medium", Size.Medium, Color.Yellow),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red, Color.Black},
                Sizes = new List<Size> {Size.Small, Size.Medium }
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }
        
        //Search which returns no results - large red
        [Test]
        public void WhenSearchingDataWithNoResults_ThenExpectedResultsAreReturned()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red},
                Sizes = new List<Size> {Size.Large}
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(shirts, searchOptions, results.ColorCounts);
        }
        
        //Search with null searchoption
        [Test]
        public void WhenSeachOptionsAreNull_ExpectedResultsAreReturned()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            SearchOptions searchOptions = null;

            Assert.Throws<ArgumentNullException>(() => searchEngine.Search(searchOptions));
        }

        //Search with no shirts
        [Test]
        public void WhenIndexingWithNullData_ThenExpectedResultsAreReturned()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchEngine(null));
        }

        [Test]
        public void WhenIndexingWithEmptyData_ThenExpectedResultsAreReturned()
        {
            Assert.Throws<ArgumentException>(() => new SearchEngine(new List<Shirt>()));
        }


        //Search with null search options (color, size)

        private static List<SearchOptions>  searchOptionsExamples = new List<SearchOptions>()
        {
            null,
            new SearchOptions(){ Colors = null, Sizes = new List<Size>()},
            new SearchOptions(){ Colors = new List<Color>(), Sizes = null},
        };
        [Test]
        [TestCaseSource("searchOptionsExamples")]
        public void IfArgumentsAreNull_ThrownsArgumentNullException(SearchOptions option)
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red)
            };
            var searchEngine = new SearchEngine(shirts);
            
            Assert.Throws<ArgumentNullException>(() => searchEngine.Search(option));

        }
        
    }
}
