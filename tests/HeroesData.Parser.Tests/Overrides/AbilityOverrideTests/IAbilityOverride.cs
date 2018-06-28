namespace HeroesData.Parser.Tests.Overrides.AbilityOverrideTests
{
    public interface IAbilityOverride
    {
        string AbilityName { get; }
        void ParentLinkOverrideTest();
        void AbilityTierOverrideTest();
        void TooltipCustomOverrideTest();
        void TooltipEnergyCostOverrideTest();
        void TooltipEnergyPerCostOverrideTest();
        void TooltipCooldownValueOverrideTest();
        void TooltipLifeCostOverrideTest();
        void TooltipIsLifePercentageOverrideTest();
    }
}
