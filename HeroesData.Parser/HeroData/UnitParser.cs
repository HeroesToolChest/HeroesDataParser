using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.HeroData.Overrides;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.HeroData
{
    public class UnitParser
    {
        private readonly int? HotsBuild;
        private readonly GameData GameData;
        private readonly OverrideData OverrideData;

        public UnitParser(GameData gameData, OverrideData overrideData)
        {
            GameData = gameData;
            OverrideData = overrideData;
        }

        public UnitParser(GameData gameData, OverrideData overrideData, int? hotsBuild)
        {
            GameData = gameData;
            OverrideData = overrideData;
            HotsBuild = hotsBuild;
        }

        public SortedSet<string> CHeroIds { get; private set; } = new SortedSet<string>();

        public void Load()
        {
            GetCHeroNames();
        }

        private void GetCHeroNames()
        {
            // CHero
            IEnumerable<XElement> cHeroElements = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            // get all heroes
            foreach (XElement hero in cHeroElements)
            {
                string id = hero.Attribute("id").Value;
                XElement attributIdValue = hero.Elements("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                if (attributIdValue == null || id == "TestHero" || id == "Random")
                    continue;

                CHeroIds.Add(id);
            }
        }
    }
}
