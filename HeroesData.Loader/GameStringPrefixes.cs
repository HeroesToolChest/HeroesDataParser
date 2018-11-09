namespace HeroesData.Loader
{
    public static class GameStringPrefixes
    {
        /// <summary>
        /// Gets the short tooltip description prefix string of the ability/talent.
        /// </summary>
        public static string SimpleDisplayPrefix { get; } = "Button/SimpleDisplayText/";

        /// <summary>
        /// Gets the short tooltip description prefix string of the ability/talent.
        /// </summary>
        public static string SimplePrefix { get; } = "Button/Simple/";

        /// <summary>
        /// Gets the description prefix string of hero.
        /// </summary>
        public static string DescriptionPrefix { get; } = "Hero/Description/";

        /// <summary>
        /// Gets the full tooltip description prefix string of the ability/talent.
        /// </summary>
        public static string FullPrefix { get; } = "Button/Tooltip/";

        /// <summary>
        /// Gets the real name prefix string of hero.
        /// </summary>
        public static string HeroNamePrefix { get; } = "Hero/Name/";

        /// <summary>
        /// Gets the real name prefix string of ability/talent.
        /// </summary>
        public static string DescriptionNamePrefix { get; } = "Button/Name/";

        /// <summary>
        /// Gets the real name prefix string of unit.
        /// </summary>
        public static string UnitPrefix { get; } = "Unit/Name/";

        /// <summary>
        /// Gets the score value prefix string.
        /// </summary>
        public static string ScoreValueTooltipPrefix { get; } = "ScoreValue/Tooltip/";

        /// <summary>
        /// Gets the award gamelink value prefix string.
        /// </summary>
        public static string MatchAwardMapSpecificInstanceNamePrefix { get; } = "UserData/EndOfMatchMapSpecificAward/";

        /// <summary>
        /// Gets the award gamelink value prefix string.
        /// </summary>
        public static string MatchAwardInstanceNamePrefix { get; } = "ScoreValue/Name/";
    }
}
