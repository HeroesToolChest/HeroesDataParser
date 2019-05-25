using Heroes.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace HeroesData.Parser.Tests.HeroDataParserTests
{
    [TestClass]
    public class GreymaneTests : HeroDataParserBaseTest
    {
        [TestMethod]
        public void WeaponTests()
        {
            IEnumerable<UnitWeapon> weapons = HeroGreymane.Weapons;

            foreach (UnitWeapon weapon in weapons)
            {
                if (weapon.WeaponNameId == "HeroGreymaneMeleeWeapon")
                {
                    Assert.AreEqual(140, weapon.Damage);
                    Assert.AreEqual(1.5, weapon.Range);
                }
            }
        }
    }
}
