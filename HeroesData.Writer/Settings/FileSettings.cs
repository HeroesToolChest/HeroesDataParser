namespace HeroesData.FileWriter.Settings
{
    internal class FileSettings
    {
        public bool IsFileSplit { get; set; } = false;
        public int DescriptionType { get; set; } = 5;
        public string ImageExtension { get; set; } = ".png";
        public bool IncludeWeapons { get; set; } = true;
        public bool IncludeAbilities { get; set; } = true;
        public bool IncludeSubAbilities { get; set; } = true;
        public bool IncludeTalents { get; set; } = true;
        public bool IncludeHeroUnits { get; set; } = true;
        public bool HeroSelectPortrait { get; set; } = true;
        public bool LeaderboardPortrait { get; set; } = true;
        public bool LoadingPortraitPortrait { get; set; } = true;
        public bool PartyPanelPortrait { get; set; } = true;
        public bool TargetPortrait { get; set; } = true;
    }
}
