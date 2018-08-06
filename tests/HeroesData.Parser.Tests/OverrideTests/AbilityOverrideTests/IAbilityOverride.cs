namespace HeroesData.Parser.Tests.OverrideTests.AbilityOverrideTests
{
    public interface IAbilityOverride
    {
        string AbilityName { get; }
        void ParentLinkOverrideTest();
        void AbilityTierOverrideTest();
        void AbilityTypeOverrideTest();
    }
}
