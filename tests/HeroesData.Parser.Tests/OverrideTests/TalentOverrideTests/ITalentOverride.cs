namespace HeroesData.Parser.Tests.OverrideTests.TalentOverrideTests
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
