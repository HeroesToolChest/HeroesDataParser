using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.XmlData;

namespace HeroesData.Parser
{
    public abstract class ParserBase
    {
        public ParserBase(GameData gameData, DefaultData defaultData)
        {
            GameData = gameData;
            DefaultData = defaultData;
        }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        public int? HotsBuild { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        public Localization Localization { get; set; }

        protected GameData GameData { get; }

        protected DefaultData DefaultData { get; }
    }
}
