namespace HeroesData.Parser.Tests.Overrides.WeaponOverrideTests
{
    public interface IWeaponOverride
    {
        string WeaponName { get; }
        void RangeOverrideTest();
        void DamageOverrideTest();
    }
}
