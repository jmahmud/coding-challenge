using System;
using NUnit.Framework;
using AutoFixture;
using System.Linq;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class HashCodeTests
    {
        private readonly Fixture _fixture;
        public HashCodeTests()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void GetHashCodeForColorsAreUnique()
        {
            var colorHashCodes = Color.All.Select(c => c.GetHashCode()).Distinct();
            Assert.AreEqual(colorHashCodes.Count(), Color.All.Count());

        }

        [Test]
        public void GetHashCodeForSizesAreUnique()
        {
            var sizeHashCodes = Size.All.Select(c => c.GetHashCode()).Distinct();
            Assert.AreEqual(sizeHashCodes.Count(), Size.All.Count());
        }
    }

   
}
