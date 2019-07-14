using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HeroesData.Helpers.Tests
{
    [TestClass]
    public class SringArrayComparerTests
    {
        private readonly HashSet<string[]> HashSetArray = new HashSet<string[]>(new StringArrayComparer());

        public SringArrayComparerTests()
        {
            HashSetArray.Add(new string[] { "item1", "item2" });
        }

        [TestMethod]
        public void ItemsAreEqual()
        {
            Assert.IsTrue(HashSetArray.Contains(new string[] { "item1", "item2" }));
        }

        [TestMethod]
        public void ItemsAreNotEqual()
        {
            Assert.IsFalse(HashSetArray.Contains(new string[] { "item2", "item1" }));
            Assert.IsFalse(HashSetArray.Contains(new string[] { "item2" }));
            Assert.IsFalse(HashSetArray.Contains(new string[] { "item1", "item3", "item2", "item2" }));
            Assert.IsFalse(HashSetArray.Contains(new string[] { string.Empty }));
        }
    }
}
