namespace HeroesData.Parser.Tests.Overrides.TalentOverrideTests
{
    public interface ITalentOverride
    {
        string TalentName { get; }
        void TooltipCustomOverrideTest();
        void TooltipEnergyCostOverrideTest();
        void TooltipEnergyPerCostOverrideTest();
        void TooltipCooldownValueOverrideTest();
        void TooltipLifeCostOverrideTest();
        void TooltipIsLifePercentageOverrideTest();
    }
}
