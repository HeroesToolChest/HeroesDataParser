using Heroes.Icons.Parser.Descriptions;
using Heroes.Icons.Parser.Heroes;
using Heroes.Icons.Parser.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Heroes.Icons.Parser.HeroData
{
    public class HeroParser
    {
        private DataLoader DataLoader;
        private DescriptionParser DescriptionParser;

        private Dictionary<string, DefaultHeroData> HeroHandler = new Dictionary<string, DefaultHeroData>();
        private SortedDictionary<string, string> HeroCHeroIds = new SortedDictionary<string, string>();

        public HeroParser(DataLoader dataLoader, DescriptionParser descriptionParser)
        {
            DataLoader = dataLoader;
            DescriptionParser = descriptionParser;

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
            var xmlData = DataLoader.HeroDataLoader.XmlData;

            // CHero
            var cHeroElements = xmlData.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            foreach (XElement hero in cHeroElements)
            {
                string id = hero.Attribute("id").Value;
                var withAttributId = hero.Elements("AttributeId").FirstOrDefault(x => x.Attribute("value") != null);

                if (withAttributId == null || id == "TestHero" || id == "Random")
                    continue;

                HeroCHeroIds.Add(id, string.Empty);
            }

            // CUnit
            var cUnitElements = xmlData.Root.Elements("CUnit").Where(x => x.Attribute("id") != null);

            foreach (XElement hero in cUnitElements)
            {
                string id = hero.Attribute("id").Value;
                string heroName = id.Substring(4);

                if (HeroCHeroIds.ContainsKey(heroName))
                    HeroCHeroIds[heroName] = id;
            }

            // add overrides
            foreach (var hero in DataLoader.HeroOverrideLoader.CUnitOverrideByCHero)
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
            HeroHandler.Add("Default", new DefaultData(DataLoader, DescriptionParser));
            HeroHandler.Add("Alarak", new AlarakData(DataLoader, DescriptionParser));
            HeroHandler.Add("Arthas", new ArthasData(DataLoader, DescriptionParser));
            HeroHandler.Add("Azmodan", new AzmodanData(DataLoader, DescriptionParser));
            HeroHandler.Add("Chromie", new ChromieData(DataLoader, DescriptionParser));
            HeroHandler.Add("Medic", new MedicData(DataLoader, DescriptionParser));
            HeroHandler.Add("Genji", new GenjiData(DataLoader, DescriptionParser));
            HeroHandler.Add("Kerrigan", new KerriganData(DataLoader, DescriptionParser));
        }
    }
}
