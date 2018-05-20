namespace Heroes.Icons.FileWriter.Settings
{
    internal class FileSettings
    {
        public bool WriterEnabled { get; set; }
        public bool FileSplit { get; set; }
        public int Description { get; set; }
        public int ShortTooltip { get; set; }
        public int FullTooltip { get; set; }
        public string ImageExtension { get; set; }
        public bool IncludeWeapons { get; set; }
        public bool IncludeAbilities { get; set; }
        public bool IncludeExtraAbilities { get; set; }
        public bool IncludeTalents { get; set; }
        public bool IncludeHeroUnits { get; set; }
    }
}
