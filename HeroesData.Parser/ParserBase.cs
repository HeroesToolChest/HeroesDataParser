using Heroes.Models;
using HeroesData.Helpers;
using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using HeroesData.Parser.XmlData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser
{
    public abstract class ParserBase<T, TOverride>
        where T : IExtractable
        where TOverride : IDataOverride
    {
        public ParserBase(Configuration configuration, GameData gameData, DefaultData defaultData)
        {
            Configuration = configuration;
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

        public virtual HashSet<string[]> Items
        {
            get
            {
                HashSet<string[]> items = new HashSet<string[]>(new StringArrayComparer());
                IEnumerable<XElement> elements = GameData.Elements(ElementType).Where(x => x.Attribute("id") != null && x.Attribute("default") == null);

                foreach (XElement element in elements)
                {
                    string id = element.Attribute("id").Value;

                    if ((ValidItem(element) && !Configuration.RemoveDataXmlElementIds(ElementType).Contains(id)) || Configuration.AddDataXmlElementIds(ElementType).Contains(id))
                        items.Add(new string[] { id });
                }

                return items;
            }
        }

        protected abstract string ElementType { get; }

        protected GameData GameData { get; }

        protected DefaultData DefaultData { get; }

        protected Configuration Configuration { get; }

        /// <summary>
        /// Applies the additional overrides. This method is called by <see cref="ApplyOverrides(T, TOverride)"/>.
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

        protected abstract bool ValidItem(XElement element);
    }
}
