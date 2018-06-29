namespace HeroesData.Parser.Tests.Overrides.PortraitOverrideTests
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
