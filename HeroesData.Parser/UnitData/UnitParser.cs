using HeroesData.Parser.GameStrings;
using HeroesData.Parser.Models;
using HeroesData.Parser.UnitData.Overrides;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData
{
    public class UnitParser
    {
        private readonly GameData GameData;
        private readonly GameStringData GameStringData;
        private readonly GameStringParser GameStringParser;
        private readonly OverrideData OverrideData;

        private SortedDictionary<string, string> CUnitIdByHeroCHeroIds = new SortedDictionary<string, string>();

        private UnitParser(GameData gameData, GameStringData gameStringData, GameStringParser gameStringParser, OverrideData overrideData)
        {
            GameData = gameData;
            GameStringData = gameStringData;
            GameStringParser = gameStringParser;
            OverrideData = overrideData;

            Initialize();
        }

        /// <summary>
        /// Gets a list of successfully parsed hero data.
        /// </summary>
        public List<Hero> ParsedHeroes { get; } = new List<Hero>(101);

        /// <summary>
        /// Gets a dictionary of heroes that unsuccesfully parsed.
        /// </summary>
        public ConcurrentDictionary<string, Exception> FailedHeroesExceptionsByHeroName { get; } = new ConcurrentDictionary<string, Exception>();

        /// <summary>
        /// Loads all unit data.
        /// </summary>
        /// <param name="gameData"></param>
        /// <param name="gameStringData"></param>
        /// <param name="gameStringParser"></param>
        /// <param name="overrideData"></param>
        /// <returns></returns>
        public static UnitParser Load(GameData gameData, GameStringData gameStringData, GameStringParser gameStringParser, OverrideData overrideData)
        {
            return new UnitParser(gameData, gameStringData, gameStringParser, overrideData);
        }

        private void Initialize()
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

        private void ParseHeroData()
        {
            Parallel.ForEach(CUnitIdByHeroCHeroIds, hero =>
            {
                try
                {
                    HeroDataParser heroDataParser = new HeroDataParser(GameData, GameStringData, GameStringParser, OverrideData);
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
