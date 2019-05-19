using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Tests.UnitParserTests
{
    [TestClass]
    public class OverwatchDataMercDefenderMeleeBruiserTests : UnitParserBaseTest
    {
        [TestMethod]
        public void PropertiesTests()
        {
            Assert.AreEqual("overwatchdata-MercDefenderMeleeBruiser", OverwatchDataMercDefenderMeleeBruiser.Id);
            Assert.AreEqual(1320, OverwatchDataMercDefenderMeleeBruiser.Life.LifeMax);
        }

        [TestMethod]
        public void WeaponTests()
        {
            List<UnitWeapon> unitWeapons = OverwatchDataMercDefenderMeleeBruiser.Weapons.ToList();

            Assert.AreEqual(14, unitWeapons[0].Damage);
        }
    }
}
