using Heroes.Models;
using Xunit;

namespace HeroesData.Parser.Tests.HeroParserTests
{
    public class GreymaneTests : HeroDataBaseTest
    {
        [Fact]
        public void WeaponTests()
        {
            var weapons = HeroGreymane.Weapons;

            foreach (UnitWeapon weapon in weapons)
            {
                if (weapon.WeaponNameId == "HeroGreymaneMeleeWeapon")
                {
                    Assert.Equal(140, weapon.Damage);
                    Assert.Equal(1.5, weapon.Range);
                }
            }
        }
    }
}
