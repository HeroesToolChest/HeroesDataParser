namespace HeroesData.FileWriter.Settings
{
    internal class FileSettings
    {
        public bool IsWriterEnabled { get; set; }
        public bool IsFileSplit { get; set; }
        public int Description { get; set; }
        public int ShortTooltip { get; set; }
        public int FullTooltip { get; set; }
        public string ImageExtension { get; set; }
        public bool IncludeWeapons { get; set; }
        public bool IncludeAbilities { get; set; }
        public bool IncludeSubAbilities { get; set; }
        public bool IncludeTalents { get; set; }
        public bool IncludeHeroUnits { get; set; }
        public bool HeroSelectPortrait { get; set; }
        public bool LeaderboardPortrait { get; set; }
        public bool LoadingPortraitPortrait { get; set; }
        public bool PartyPanelPortrait { get; set; }
        public bool TargetPortrait { get; set; }
    }
}
