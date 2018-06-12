using HeroesData.Parser.Models;
using HeroesData.Parser.XmlGameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.UnitData.Overrides
{
    public class OverrideData
    {
        private readonly GameData GameData;
        private readonly int? HotsBuild;

        private Dictionary<string, HeroOverride> HeroOverridesByCHeroId = new Dictionary<string, HeroOverride>();

        private OverrideData(GameData gameData)
        {
            GameData = gameData;
            Initialize();
        }

        private OverrideData(GameData gameData, int? hotsBuild)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;
            Initialize();
        }

        /// <summary>
        /// Gets the file name of the Override file.
        /// </summary>
        public string HeroDataOverrideXmlFile { get; private set; } = @"HeroOverrides.xml";

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData)
        {
            return new OverrideData(gameData);
        }

        /// <summary>
        /// Loads the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <returns></returns>
        public static OverrideData Load(GameData gameData, int? hotsBuild)
        {
            return new OverrideData(gameData, hotsBuild);
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
            if (HotsBuild.HasValue)
            {
                string file = $"{Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile)}_{HotsBuild}.xml";

                if (File.Exists(file))
                {
                    HeroDataOverrideXmlFile = file;
                    return XDocument.Load(file);
                }
            }

            // default load
            if (File.Exists(HeroDataOverrideXmlFile))
            {
                return XDocument.Load(HeroDataOverrideXmlFile);
            }
            else
            {
                if (HotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {HeroDataOverrideXmlFile} or {Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile)}_{HotsBuild}.xml");
                else
                    throw new FileNotFoundException($"File not found: {HeroDataOverrideXmlFile}");
            }
        }

        private void SetHeroOverrides(XElement heroElement)
        {
            HeroOverride heroOverride = new HeroOverride();
            AbilityOverride abilityOverride = new AbilityOverride(GameData, HotsBuild);
            WeaponOverride weaponOverride = new WeaponOverride(GameData, HotsBuild);
            PortraitOverride portraitOverride = new PortraitOverride(GameData, HotsBuild);

            string cHeroId = heroElement.Attribute("id").Value;
            string imageAlt = heroElement.Attribute("alt")?.Value;

            foreach (var dataElement in heroElement.Elements())
            {
                string elementName = dataElement.Name.LocalName;

                switch (elementName)
                {
                    case "Name":
                        heroOverride.NameOverride = (true, dataElement.Attribute("value").Value);
                        break;
                    case "ShortName":
                        heroOverride.ShortNameOverride = (true, dataElement.Attribute("value").Value);
                        break;
                    case "CUnit":
                        heroOverride.CUnitOverride = (true, dataElement.Attribute("value").Value);
                        break;
                    case "EnergyType":
                        string energyType = dataElement.Attribute("value").Value;
                        if (Enum.TryParse(energyType, out UnitEnergyType heroEnergyType))
                            heroOverride.EnergyTypeOverride = (true, heroEnergyType);
                        else
                            heroOverride.EnergyTypeOverride = (true, UnitEnergyType.None);
                        break;
                    case "Energy":
                        string energyValue = dataElement.Attribute("value").Value;
                        if (int.TryParse(energyValue, out int value))
                            heroOverride.EnergyOverride = (true, value);
                        else
                            heroOverride.EnergyOverride = (true, 0);
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
                        heroOverride.ParentLinkOverride = (true, dataElement.Attribute("value").Value);
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
