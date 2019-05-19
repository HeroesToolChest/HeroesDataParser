﻿using Heroes.Models;
using Heroes.Models.AbilityTalents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class RagnarosTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void HeroUnitTests()
        {
            Assert.AreEqual(1, HeroRagnaros.HeroUnits.Count);

            Unit unit = HeroRagnaros.HeroUnits[0];
            Assert.AreEqual("RagnarosBigRag", unit.CUnitId);
            Assert.AreEqual("RagnarosBigRag", unit.HyperlinkId);
            Assert.AreEqual("Ragnaros", unit.Name);

            Ability ability = unit.GetAbility("RagnarosBigRagMeteorShower");
            Assert.AreEqual("Meteor Shower", ability.Name);
            Assert.AreEqual(AbilityType.W, ability.AbilityType);
        }

        [TestMethod]
        public void HeroDescriptorsTests()
        {
            Assert.AreEqual(5, HeroRagnaros.HeroDescriptors.Count());

            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("EnergyImportant"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Escaper"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("Overconfident"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("RoleCaster"));
            Assert.IsTrue(HeroRagnaros.HeroDescriptors.Contains("WaveClearer"));
        }
    }
}
