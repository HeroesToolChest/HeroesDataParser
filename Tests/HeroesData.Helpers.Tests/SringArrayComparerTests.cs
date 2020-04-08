using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HeroesData.Helpers.Tests
{
    [TestClass]
    public class SringArrayComparerTests
    {
        private readonly HashSet<string[]> _hashSetArray = new HashSet<string[]>(new StringArrayComparer());

        public SringArrayComparerTests()
        {
            _hashSetArray.Add(new string[] { "item1", "item2" });
        }

        [TestMethod]
        public void ItemsAreEqual()
        {
            Assert.IsTrue(_hashSetArray.Contains(new string[] { "item1", "item2" }));
        }

        [TestMethod]
        public void ItemsAreNotEqual()
        {
            Assert.IsFalse(_hashSetArray.Contains(new string[] { "item2", "item1" }));
            Assert.IsFalse(_hashSetArray.Contains(new string[] { "item2" }));
            Assert.IsFalse(_hashSetArray.Contains(new string[] { "item1", "item3", "item2", "item2" }));
            Assert.IsFalse(_hashSetArray.Contains(new string[] { string.Empty }));
        }
    }
}
