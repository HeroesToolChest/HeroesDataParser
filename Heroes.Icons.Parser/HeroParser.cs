using Heroes.Icons.Parser.Heroes;
using Heroes.Icons.Parser.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heroes.Icons.Parser
{
    public class HeroParser
    {
        private HeroDataLoader HeroDataLoader;
        private DescriptionLoader DescriptionLoader;
        private DescriptionParser DescriptionParser;
        private HeroOverrideLoader HeroOverrideLoader;

        private Dictionary<string, HeroData> HeroHandler = new Dictionary<string, HeroData>();
        private SortedDictionary<string, string> HeroCHeroIds = new SortedDictionary<string, string>();

        public HeroParser(HeroDataLoader heroDataLoader, DescriptionLoader descriptionLoader, DescriptionParser descriptionParser, HeroOverrideLoader heroOverrideLoader)
        {
            HeroDataLoader = heroDataLoader;
            DescriptionLoader = descriptionLoader;
            DescriptionParser = descriptionParser;
            HeroOverrideLoader = heroOverrideLoader;

            SetHeroHandler();
        }

        /// <summary>
        /// A list of successfully parsed hero data
        /// </summary>
        public List<Hero> ParsedHeroes { get; set; } = new List<Hero>();

        /// <summary>
        /// Heroes that were unsucessfully parsed.
        /// Key: Hero name
        /// Value: Error Message
        /// </summary>
        public ConcurrentDictionary<string, Exception> FailedHeroes { get; set; } = new ConcurrentDictionary<string, Exception>();

        public void Parse()
        {
            GetCHeroNames();
            ParseHeroData();
        }

        private void GetCHeroNames()
        {
            var data = HeroDataLoader.XmlData;

            // CHero
            var cHeroElements = data.Descendants("CHero").Where(x => x.Attribute("id") != null);

            foreach (var hero in cHeroElements)
            {
                string id = hero.Attribute("id").Value;
                var withAttributId = hero.Descendants("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                if (withAttributId == null || id == "TestHero" || id == "Random")
                    continue;

                HeroCHeroIds.Add(id, string.Empty);
            }

            // CUnit
            var cUnitElements = data.Descendants("CUnit").Where(x => x.Attribute("id") != null);

            foreach (var hero in cUnitElements)
            {
                string id = hero.Attribute("id").Value;
                string heroName = id.Substring(4);

                if (HeroCHeroIds.ContainsKey(heroName))
                    HeroCHeroIds[heroName] = id;
            }

            // add overrides
            foreach (var hero in HeroOverrideLoader.CUnitOverrideByCHero)
            {
                if (HeroCHeroIds.ContainsKey(hero.Key))
                    HeroCHeroIds[hero.Key] = hero.Value;
            }
        }

        /// <summary>
        /// Get all data associated with the hero
        /// </summary>
        private void ParseHeroData()
        {
            Parallel.ForEach(HeroCHeroIds, hero =>
            {
                try
                {
                    if (HeroHandler.ContainsKey(hero.Key))
                        ParsedHeroes.Add(HeroHandler[hero.Key].ParseHeroData(hero.Key, hero.Value));
                    else
                        ParsedHeroes.Add(HeroHandler["Default"].ParseHeroData(hero.Key, hero.Value));
                }
                catch (Exception ex)
                {
                    FailedHeroes.GetOrAdd(hero.Key, ex);
                    return;
                }
            });
        }

        // only need to be set for heroes that inherit/override HeroData
        private void SetHeroHandler()
        {
            HeroHandler.Add("Default", new DefaultData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Alarak", new AlarakData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Arthas", new ArthasData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Azmodan", new AzmodanData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Chromie", new ChromieData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Medic", new MedicData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Genji", new GenjiData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
            HeroHandler.Add("Kerrigan", new KerriganData(HeroDataLoader, DescriptionLoader, DescriptionParser, HeroOverrideLoader));
        }
    }
}
