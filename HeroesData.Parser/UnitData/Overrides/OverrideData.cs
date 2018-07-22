using Heroes.Models;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class OverrideData
    {
        private readonly GameData GameData;
        private readonly int? HotsBuild;
        private readonly string HeroOverridesDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "HeroOverridesXml");

        private Dictionary<string, HeroOverride> HeroOverridesByCHeroId = new Dictionary<string, HeroOverride>();

        private OverrideData(GameData gameData, string heroDataOverrideFile)
        {
            GameData = gameData;

            if (!string.IsNullOrEmpty(heroDataOverrideFile))
                HeroDataOverrideXmlFile = heroDataOverrideFile;

            Initialize();
        }

        private OverrideData(GameData gameData, int? hotsBuild, string heroDataOverrideFile)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;

            if (!string.IsNullOrEmpty(heroDataOverrideFile))
                HeroDataOverrideXmlFile = heroDataOverrideFile;

            Initialize();
        }

        /// <summary>
        /// Gets the file name of the Override file.
        /// </summary>
        public string HeroDataOverrideXmlFile { get; private set; } = "HeroOverrides.xml";

        /// <summary>
        /// Gets the total number of overrides.
        /// </summary>
        public int Count => HeroOverridesByCHeroId.Count;

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData)
        {
            return new OverrideData(gameData, null);
        }

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="heroDataOverrideFile">The file name of the overrides file.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData, string heroDataOverrideFile)
        {
            return new OverrideData(gameData, heroDataOverrideFile);
        }

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData, int? hotsBuild)
        {
            return new OverrideData(gameData, hotsBuild, null);
        }

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <param name="heroDataOverrideFile">The file name of the overrides file.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData, int? hotsBuild, string heroDataOverrideFile)
        {
            return new OverrideData(gameData, hotsBuild, heroDataOverrideFile);
        }

        /// <summary>
        /// Gets the HeroOverride for the given cHeroId. Returns null if none found.
        /// </summary>
        /// <param name="cHeroId">CHero id of hero name.</param>
        /// <returns></returns>
        public HeroOverride HeroOverride(string cHeroId)
        {
            if (HeroOverridesByCHeroId.TryGetValue(cHeroId, out HeroOverride overrideData))
                return overrideData;
            else
                return null;
        }

        private void Initialize()
        {
            XDocument cHeroDocument = LoadOverrideFile();
            IEnumerable<XElement> cHeroes = cHeroDocument.Root.Elements("CHero").Where(x => x.Attribute("id") != null);

            foreach (XElement heroElement in cHeroes)
            {
                SetHeroOverrides(heroElement);
            }
        }

        private XDocument LoadOverrideFile()
        {
            // see if we can load a build override file
            if (HotsBuild.HasValue)
            {
                string file = string.Empty;
                string fileNoExtension = Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile);

                if (fileNoExtension.EndsWith(HotsBuild.Value.ToString()))
                    file = HeroDataOverrideXmlFile;
                else
                    file = Path.Combine(Path.GetDirectoryName(HeroDataOverrideXmlFile), $"{fileNoExtension}_{HotsBuild}.xml");

                // check if exact build number override file exists
                if (File.Exists(Path.Combine(HeroOverridesDirectoryPath, file)))
                {
                    HeroDataOverrideXmlFile = file;
                    return XDocument.Load(Path.Combine(HeroOverridesDirectoryPath, file));
                }
                else // load the next lowest build override file
                {
                    (int lowestBuild, int highestBuild, int selectedBuild, int difference) = (int.MaxValue, int.MinValue, HotsBuild.Value, 999999);

                    foreach (string directoryName in Directory.EnumerateFiles(HeroOverridesDirectoryPath, $"{Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile)}_*.xml"))
                    {
                        if (int.TryParse(Path.GetFileNameWithoutExtension(directoryName).Split('_').LastOrDefault(), out int buildNumber))
                        {
                            if (buildNumber > highestBuild)
                                highestBuild = buildNumber;

                            if (buildNumber < lowestBuild)
                                lowestBuild = buildNumber;

                            if (HotsBuild.Value - buildNumber > 0 && (HotsBuild.Value - buildNumber < difference))
                            {
                                selectedBuild = buildNumber;
                                difference = HotsBuild.Value - buildNumber;
                            }
                        }
                    }

                    // if the build is lower than the lowest override build file, then load that override build file
                    if (HotsBuild.Value < lowestBuild)
                    {
                        // check if it exists and load it
                        file = Path.Combine(Path.GetDirectoryName(HeroDataOverrideXmlFile), $"{fileNoExtension}_{lowestBuild}.xml");

                        if (File.Exists(Path.Combine(HeroOverridesDirectoryPath, file)))
                        {
                            HeroDataOverrideXmlFile = file;
                            return XDocument.Load(Path.Combine(HeroOverridesDirectoryPath, file));
                        }
                    }

                    if (HotsBuild.Value < highestBuild)
                    {
                        // check if it exists and load it
                        file = Path.Combine(Path.GetDirectoryName(HeroDataOverrideXmlFile), $"{fileNoExtension}_{selectedBuild}.xml");

                        if (File.Exists(Path.Combine(HeroOverridesDirectoryPath, file)))
                        {
                            HeroDataOverrideXmlFile = file;
                            return XDocument.Load(Path.Combine(HeroOverridesDirectoryPath, file));
                        }
                    }
                }
            }

            // default load
            if (File.Exists(Path.Combine(HeroOverridesDirectoryPath, HeroDataOverrideXmlFile)))
            {
                return XDocument.Load(Path.Combine(HeroOverridesDirectoryPath, HeroDataOverrideXmlFile));
            }
            else
            {
                if (HotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {HeroDataOverrideXmlFile} or {Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile)}_{HotsBuild}.xml at {HeroOverridesDirectoryPath}");
                else
                    throw new FileNotFoundException($"File not found: {HeroDataOverrideXmlFile} at {HeroOverridesDirectoryPath}");
            }
        }

        private void SetHeroOverrides(XElement heroElement)
        {
            HeroOverride heroOverride = new HeroOverride();
            AbilityOverride abilityOverride = new AbilityOverride(GameData, HotsBuild);
            TalentOverride talentOverride = new TalentOverride(GameData, HotsBuild);
            WeaponOverride weaponOverride = new WeaponOverride(GameData, HotsBuild);
            PortraitOverride portraitOverride = new PortraitOverride(GameData, HotsBuild);

            string cHeroId = heroElement.Attribute("id").Value;
            string imageAlt = heroElement.Attribute("alt")?.Value;

            foreach (var dataElement in heroElement.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value;
                switch (elementName)
                {
                    case "Name":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            heroOverride.NameOverride = (true, valueAttribute);
                        break;
                    case "ShortName":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            heroOverride.ShortNameOverride = (true, valueAttribute);
                        break;
                    case "CUnit":
                        if (!string.IsNullOrEmpty(valueAttribute))
                            heroOverride.CUnitOverride = (true, valueAttribute);
                        break;
                    case "EnergyType":
                        string energyType = valueAttribute;
                        if (Enum.TryParse(energyType, out UnitEnergyType heroEnergyType))
                            heroOverride.EnergyTypeOverride = (true, heroEnergyType);
                        else
                            heroOverride.EnergyTypeOverride = (true, UnitEnergyType.None);
                        break;
                    case "Energy":
                        string energyValue = valueAttribute;
                        if (int.TryParse(energyValue, out int value))
                        {
                            if (value < 0)
                                value = 0;

                            heroOverride.EnergyOverride = (true, value);
                        }
                        else
                        {
                            heroOverride.EnergyOverride = (true, 0);
                        }

                        break;
                    case "Ability":
                        string abilityId = dataElement.Attribute("id")?.Value;
                        string valid = dataElement.Attribute("valid")?.Value;
                        string add = dataElement.Attribute("add")?.Value;
                        string button = dataElement.Attribute("button")?.Value;

                        if (string.IsNullOrEmpty(abilityId))
                            continue;

                        // valid
                        if (bool.TryParse(valid, out bool result))
                        {
                            heroOverride.IsValidAbilityByAbilityId.Add(abilityId, result);

                            if (!result)
                                continue;
                        }

                        // add
                        if (bool.TryParse(add, out result))
                        {
                            heroOverride.AddedAbilitiesByAbilityId.Add(abilityId, (button, result));

                            if (!result)
                                continue;
                        }

                        // override
                        var overrideElement = dataElement.Elements("Override").FirstOrDefault();
                        if (overrideElement != null)
                            abilityOverride.SetOverride(abilityId, overrideElement, heroOverride.PropertyAbilityOverrideMethodByAbilityId);
                        break;
                    case "Talent":
                        string talentId = dataElement.Attribute("id")?.Value;
                        valid = dataElement.Attribute("valid")?.Value;
                        add = dataElement.Attribute("add")?.Value;
                        button = dataElement.Attribute("button")?.Value;

                        if (string.IsNullOrEmpty(talentId))
                            continue;

                        // override
                        overrideElement = dataElement.Elements("Override").FirstOrDefault();
                        if (overrideElement != null)
                            talentOverride.SetOverride(talentId, overrideElement, heroOverride.PropertyTalentOverrideMethodByTalentId);
                        break;
                    case "LinkedAbilities":
                        SetLinkAbilities(dataElement, heroOverride);
                        break;
                    case "Weapon":
                        string weaponId = dataElement.Attribute("id")?.Value;
                        valid = dataElement.Attribute("valid")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(valid, out result))
                        {
                            heroOverride.IsValidWeaponByWeaponId.Add(weaponId, result);

                            if (!result)
                                continue;
                        }

                        overrideElement = dataElement.Elements("Override").FirstOrDefault();
                        if (overrideElement != null)
                            weaponOverride.SetOverride(weaponId, overrideElement, heroOverride.PropertyWeaponOverrideMethodByWeaponId);
                        break;
                    case "HeroUnit":
                        string heroUnitId = dataElement.Attribute("id")?.Value;

                        if (string.IsNullOrEmpty(heroUnitId))
                            continue;

                        AddHeroUnits(heroUnitId, dataElement, heroOverride);
                        break;
                    case "ParentLink":
                        heroOverride.ParentLinkOverride = (true, dataElement.Attribute("value")?.Value);
                        break;
                    case "Portrait":
                        overrideElement = dataElement.Elements("Override").FirstOrDefault();
                        if (overrideElement != null)
                            portraitOverride.SetOverride(cHeroId, overrideElement, heroOverride.PropertyPortraitOverrideMethodByCHeroId);
                        break;
                }
            }

            if (!string.IsNullOrEmpty(imageAlt))
                SetPortraits(cHeroId, imageAlt, heroOverride);

            if (!HeroOverridesByCHeroId.ContainsKey(cHeroId))
                HeroOverridesByCHeroId.Add(cHeroId, heroOverride);
        }

        private void SetLinkAbilities(XElement element, HeroOverride heroOverride)
        {
            foreach (var linkAbility in element.Elements())
            {
                string id = linkAbility.Attribute("id")?.Value;
                string elementName = linkAbility.Attribute("element")?.Value;
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(elementName))
                    continue;

                // add or update
                heroOverride.LinkedElementNamesByAbilityId[id] = elementName;
            }
        }

        private void SetPortraits(string cHeroId, string imageAlt, HeroOverride heroOverride)
        {
            imageAlt = imageAlt.ToLower();

            var propertyOverrides = new Dictionary<string, Action<HeroPortrait>>();

            if (heroOverride.PropertyPortraitOverrideMethodByCHeroId.ContainsKey(cHeroId))
            {
                if (!heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].ContainsKey(nameof(Hero.HeroPortrait.HeroSelectPortraitFileName)))
                {
                    heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].Add(nameof(Hero.HeroPortrait.HeroSelectPortraitFileName), (portrait) =>
                    {
                        portrait.HeroSelectPortraitFileName = $"{PortraitFileNames.HeroSelectPortraitPrefix}{imageAlt}.dds";
                    });
                }

                if (!heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].ContainsKey(nameof(Hero.HeroPortrait.LeaderboardPortraitFileName)))
                {
                    heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].Add(nameof(Hero.HeroPortrait.LeaderboardPortraitFileName), (portrait) =>
                    {
                        portrait.LeaderboardPortraitFileName = $"{PortraitFileNames.LeaderboardPortraitPrefix}{imageAlt}.dds";
                    });
                }

                if (!heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].ContainsKey(nameof(Hero.HeroPortrait.LoadingScreenPortraitFileName)))
                {
                    heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].Add(nameof(Hero.HeroPortrait.LoadingScreenPortraitFileName), (portrait) =>
                    {
                        portrait.LoadingScreenPortraitFileName = $"{PortraitFileNames.LoadingPortraitPrefix}{imageAlt}.dds";
                    });
                }

                if (!heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].ContainsKey(nameof(Hero.HeroPortrait.PartyPanelPortraitFileName)))
                {
                    heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].Add(nameof(Hero.HeroPortrait.PartyPanelPortraitFileName), (portrait) =>
                    {
                        portrait.PartyPanelPortraitFileName = $"{PortraitFileNames.PartyPanelPortraitPrefix}{imageAlt}.dds";
                    });
                }

                if (!heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].ContainsKey(nameof(Hero.HeroPortrait.TargetPortraitFileName)))
                {
                    heroOverride.PropertyPortraitOverrideMethodByCHeroId[cHeroId].Add(nameof(Hero.HeroPortrait.TargetPortraitFileName), (portrait) =>
                    {
                        portrait.TargetPortraitFileName = $"{PortraitFileNames.TargetPortraitPrefix}{imageAlt}.dds";
                    });
                }
            }
            else
            {
                propertyOverrides.Add(nameof(Hero.HeroPortrait.HeroSelectPortraitFileName), (portrait) =>
                {
                    portrait.HeroSelectPortraitFileName = $"{PortraitFileNames.HeroSelectPortraitPrefix}{imageAlt}.dds";
                });
                propertyOverrides.Add(nameof(Hero.HeroPortrait.LeaderboardPortraitFileName), (portrait) =>
                {
                    portrait.LeaderboardPortraitFileName = $"{PortraitFileNames.LeaderboardPortraitPrefix}{imageAlt}.dds";
                });
                propertyOverrides.Add(nameof(Hero.HeroPortrait.LoadingScreenPortraitFileName), (portrait) =>
                {
                    portrait.LoadingScreenPortraitFileName = $"{PortraitFileNames.LoadingPortraitPrefix}{imageAlt}.dds";
                });
                propertyOverrides.Add(nameof(Hero.HeroPortrait.PartyPanelPortraitFileName), (portrait) =>
                {
                    portrait.PartyPanelPortraitFileName = $"{PortraitFileNames.PartyPanelPortraitPrefix}{imageAlt}.dds";
                });
                propertyOverrides.Add(nameof(Hero.HeroPortrait.TargetPortraitFileName), (portrait) =>
                {
                    portrait.TargetPortraitFileName = $"{PortraitFileNames.TargetPortraitPrefix}{imageAlt}.dds";
                });

                heroOverride.PropertyPortraitOverrideMethodByCHeroId.Add(cHeroId, propertyOverrides);
            }
        }

        private void AddHeroUnits(string elementId, XElement element, HeroOverride heroOverride)
        {
            heroOverride.HeroUnits.Add(elementId);
            SetHeroOverrides(element);
        }
    }
}
