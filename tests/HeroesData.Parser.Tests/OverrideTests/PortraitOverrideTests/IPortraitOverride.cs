namespace HeroesData.Parser.Tests.OverrideTests.PortraitOverrideTests
{
    public interface IPortraitOverride
    {
        string CHeroIdName { get; }
        void HeroSelectPortraitOverrideTest();
        void LeaderboardPortraitOverrideTest();
        void LoadingScreenPortraitOverrideTest();
        void PartyPanelPortraitOverrideTest();
        void TargetPortraitOverrideTest();
    }
}
