using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData
{
    public class UnitParser
    {
        private readonly int? HotsBuild;
        private readonly GameData GameData;
        private readonly OverrideData OverrideData;

        private UnitParser(GameData gameData, OverrideData overrideData)
        {
            GameData = gameData;
            OverrideData = overrideData;

            Initialize();
        }

        private UnitParser(GameData gameData, OverrideData overrideData, int? hotsBuild)
        {
            GameData = gameData;
            OverrideData = overrideData;
            HotsBuild = hotsBuild;

            Initialize();
        }

        public SortedDictionary<string, string> CUnitIdByHeroCHeroIds { get; private set; } = new SortedDictionary<string, string>();

        /// <summary>
        /// Loads all unit id data to be parsed.
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="gameStringParser"></param>
        /// <param name="overrideData"></param>
        /// <returns></returns>
        public static UnitParser Load(GameData gameData, OverrideData overrideData)
        {
            return new UnitParser(gameData, overrideData);
        }

        /// <summary>
        /// Loads all unit id data to be parsed.
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="gameStringParser"></param>
        /// <param name="overrideData"></param>
        /// <param name="hotsBuild">The hots build number.</param>
        /// <returns></returns>
        public static UnitParser Load(GameData gameData, OverrideData overrideData, int? hotsBuild)
        {
            return new UnitParser(gameData, overrideData, hotsBuild);
        }

        private void Initialize()
        {
            GetCHeroNames();
        }

        private void GetCHeroNames()
        {
            // CHero
            var cHeroElements = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            // get all heroes
            foreach (XElement hero in cHeroElements)
            {
                string id = hero.Attribute("id").Value;
                var withAttributId = hero.Elements("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                if (withAttributId == null || id == "TestHero" || id == "Random")
                    continue;

                CUnitIdByHeroCHeroIds.Add(id, string.Empty);
            }

            // get all hero cunit id and associate it with the chero id found above
            var cUnitElements = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id") != null);

            foreach (XElement hero in cUnitElements)
            {
                string id = hero.Attribute("id").Value;
                string heroName = id.Substring(4); // names start with Hero

                if (CUnitIdByHeroCHeroIds.ContainsKey(heroName))
                    CUnitIdByHeroCHeroIds[heroName] = id;
            }

            // add overrides for CUnit
            foreach (KeyValuePair<string, string> hero in CUnitIdByHeroCHeroIds.ToList())
            {
                HeroOverride heroOverride = OverrideData.HeroOverride(hero.Key);
                if (heroOverride != null)
                {
                    if (heroOverride.CUnitOverride.Enabled)
                        CUnitIdByHeroCHeroIds[hero.Key] = heroOverride.CUnitOverride.CUnit;
                }
            }
        }
    }
}
