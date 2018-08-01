namespace HeroesData.Parser.Tests.OverrideTests.WeaponOverrideTests
{
    public interface IWeaponOverride
    {
        string WeaponName { get; }
        void RangeOverrideTest();
        void DamageOverrideTest();
    }
}
