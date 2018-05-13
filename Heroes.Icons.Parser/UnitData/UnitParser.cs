using Heroes.Icons.Parser.GameStrings;
using Heroes.Icons.Parser.Models;
using Heroes.Icons.Parser.UnitData.Overrides;
using Heroes.Icons.Parser.XmlGameData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.UnitData
{
    public class UnitParser
    {
        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly GameStringParser GameStringParser;
        private readonly HeroOverrideData HeroOverrideData;

        private SortedDictionary<string, string> CUnitIdByHeroCHeroIds = new SortedDictionary<string, string>();

        public UnitParser(GameData gameData, GameStringData gameStringData, GameStringParser gameStringParser, HeroOverrideData heroOverrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            GameStringParser = gameStringParser;
            HeroOverrideData = heroOverrideData;
        }

        /// <summary>
        /// Gets or sets a list of successfully parsed hero data
        /// </summary>
        public List<Hero> ParsedHeroes { get; set; } = new List<Hero>(101);

        /// <summary>
        /// Gets or sets a dictionary of heroes that unsuccesfully parsed
        /// </summary>
        public ConcurrentDictionary<string, Exception> FailedHeroesExceptionsByHeroName { get; set; } = new ConcurrentDictionary<string, Exception>();

        public void ParseHeroes()
        {
            GetCHeroNames();
            ParseHeroData();
        }

        private void GetCHeroNames()
        {
            // CHero
            var cHeroElements = GameData.XmlGameData.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            foreach (XElement hero in cHeroElements)
            {
                string id = hero.Attribute("id").Value;
                var withAttributId = hero.Elements("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                if (withAttributId == null || id == "TestHero" || id == "Random")
                    continue;

                CUnitIdByHeroCHeroIds.Add(id, string.Empty);
            }

            // CUnit
            var cUnitElements = GameData.XmlGameData.Root.Elements("CUnit").Where(x => x.Attribute("id") != null);

            foreach (XElement hero in cUnitElements)
            {
                string id = hero.Attribute("id").Value;
                string heroName = id.Substring(4); // names start with Hero

                if (CUnitIdByHeroCHeroIds.ContainsKey(heroName))
                    CUnitIdByHeroCHeroIds[heroName] = id;
            }

            // add overrides for CUnit
            foreach (KeyValuePair<string, HeroOverride> heroOverride in HeroOverrideData.HeroOverridesByCHero)
            {
                if (CUnitIdByHeroCHeroIds.ContainsKey(heroOverride.Key))
                {
                    if (heroOverride.Value.CUnitOverride.Enabled)
                        CUnitIdByHeroCHeroIds[heroOverride.Key] = heroOverride.Value.CUnitOverride.CUnit;
                }
            }
        }

        private void ParseHeroData()
        {
            Parallel.ForEach(CUnitIdByHeroCHeroIds, hero =>
            {
                try
                {
                    HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, GameStringParser, HeroOverrideData);
                    ParsedHeroes.Add(heroDataParser.Parse(hero.Key, hero.Value));
                }
                catch (Exception ex)
                {
                    FailedHeroesExceptionsByHeroName.GetOrAdd(hero.Key, ex);
                    return;
                }
            });
        }
    }
}
