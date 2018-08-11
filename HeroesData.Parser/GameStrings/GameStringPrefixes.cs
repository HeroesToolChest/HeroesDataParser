namespace HeroesData.Parser.GameStrings
{
    public static class GameStringPrefixes
    {
        /// <summary>
        /// Short tooltip description of the ability/talent.
        /// </summary>
        public static string SimpleDisplayPrefix { get; } = "Button/SimpleDisplayText/";

        /// <summary>
        /// Short tooltip description of the ability/talent
        /// </summary>
        public static string SimplePrefix { get; } = "Button/Simple/";

        /// <summary>
        /// Description of hero.
        /// </summary>
        public static string DescriptionPrefix { get; } = "Hero/Description/";

        /// <summary>
        /// Full tooltip description of the ability/talent.
        /// </summary>
        public static string FullPrefix { get; } = "Button/Tooltip/";

        /// <summary>
        /// Real name of hero.
        /// </summary>
        public static string HeroNamePrefix { get; } = "Hero/Name/";

        /// <summary>
        /// Real name of ability/talent.
        /// </summary>
        public static string DescriptionNamePrefix { get; } = "Button/Name/";

        /// <summary>
        /// Real name of unit.
        /// </summary>
        public static string UnitPrefix { get; } = "Unit/Name/";
    }
}
