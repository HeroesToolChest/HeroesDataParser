using Heroes.Models;
using HeroesData.Loader.XmlGameData;

namespace HeroesData.Parser
{
    public abstract class ParserBase
    {
        public ParserBase(GameData gameData)
        {
            GameData = gameData;
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
    }
}
