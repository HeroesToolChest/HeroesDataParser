using Heroes.Models;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;

namespace HeroesData.Parser
{
    public abstract class ParserBase<T, TOverride>
        where T : IExtractable
        where TOverride : IDataOverride
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

        /// <summary>
        /// Applies the additonal overrides. This method is called by <see cref="ApplyOverrides(T, TOverride)"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dataOverride"></param>
        protected virtual void ApplyAdditionalOverrides(T t, TOverride dataOverride)
        {
            return;
        }

        /// <summary>
        /// Applies the base overrides as well as additional overrides from <see cref="ApplyAdditionalOverrides(T, TOverride)"/>.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="dataOverride"></param>
        protected virtual void ApplyOverrides(T t, TOverride dataOverride)
        {
            if (dataOverride == null)
                return;

            if (dataOverride.IdOverride.Enabled)
                t.Id = dataOverride.IdOverride.Value;

            if (dataOverride.NameOverride.Enabled)
                t.Name = dataOverride.NameOverride.Value;

            if (dataOverride.HyperlinkIdOverride.Enabled)
                t.HyperlinkId = dataOverride.HyperlinkIdOverride.Value;

            ApplyAdditionalOverrides(t, dataOverride);
        }
    }
}
