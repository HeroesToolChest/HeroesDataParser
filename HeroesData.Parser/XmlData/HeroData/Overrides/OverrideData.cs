using HeroesData.Loader.XmlGameData;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlData.HeroData.Overrides
{
    public class OverrideData
    {
        private readonly GameData GameData;
        private readonly int? HotsBuild;
        private readonly string HeroOverridesDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "herooverrides");

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
        public string HeroDataOverrideXmlFile { get; private set; } = "hero-overrides.xml";

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

                    foreach (string filePath in Directory.EnumerateFiles(HeroOverridesDirectoryPath, $"{Path.GetFileNameWithoutExtension(HeroDataOverrideXmlFile)}_*.xml"))
                    {
                        if (int.TryParse(Path.GetFileNameWithoutExtension(filePath).Split('_').LastOrDefault(), out int buildNumber))
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

            foreach (XElement dataElement in heroElement.Elements())
            {
                string elementName = dataElement.Name.LocalName;
                string valueAttribute = dataElement.Attribute("value")?.Value;

                XElement overrideElement = null;

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
                        heroOverride.EnergyTypeOverride = (true, energyType);
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
                        string referenceNameId = dataElement.Attribute("referenceNameId")?.Value;
                        string remove = dataElement.Attribute("remove")?.Value;
                        string newButtonName = dataElement.Element("ButtonName")?.Attribute("value")?.Value;

                        if (!string.IsNullOrEmpty(abilityId))
                        {
                            // valid
                            if (bool.TryParse(valid, out bool validResult))
                            {
                                heroOverride.IsValidAbilityByAbilityId.Add(abilityId, validResult);

                                if (!validResult)
                                    continue;
                            }

                            // add
                            if (bool.TryParse(add, out bool addResult))
                            {
                                heroOverride.AddedAbilityByAbilityId.Add(abilityId, (button, addResult));

                                if (!addResult)
                                    continue;
                            }

                            if (!string.IsNullOrEmpty(button) && !string.IsNullOrEmpty(newButtonName))
                            {
                                heroOverride.ButtonNameOverrideByAbilityButtonId.Add((abilityId, button), newButtonName);
                            }

                            // override
                            overrideElement = dataElement.Element("Override");
                            if (overrideElement != null)
                                abilityOverride.SetOverride(abilityId, overrideElement, heroOverride.PropertyAbilityOverrideMethodByAbilityId);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(referenceNameId) && !string.IsNullOrEmpty(remove))
                            {
                                if (bool.TryParse(remove, out bool removeResult))
                                {
                                    heroOverride.RemovedAbilityByAbilityReferenceNameId.Add(referenceNameId, removeResult);
                                }

                                continue;
                            }
                        }

                        break;
                    case "Button":
                        string buttonId = dataElement.Attribute("id")?.Value;
                        string parent = dataElement.Attribute("parent")?.Value;

                        if (string.IsNullOrEmpty(buttonId))
                            continue;

                        if (parent == null)
                            parent = string.Empty;

                        heroOverride.AddedAbilityByButtonId.Add((buttonId, parent));

                        // override
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            abilityOverride.SetOverride(buttonId, overrideElement, heroOverride.PropertyAbilityOverrideMethodByAbilityId);
                        break;
                    case "Talent":
                        string talentId = dataElement.Attribute("id")?.Value;
                        valid = dataElement.Attribute("valid")?.Value;
                        add = dataElement.Attribute("add")?.Value;
                        button = dataElement.Attribute("button")?.Value;

                        if (string.IsNullOrEmpty(talentId))
                            continue;

                        // override
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            talentOverride.SetOverride(talentId, overrideElement, heroOverride.PropertyTalentOverrideMethodByTalentId);
                        break;
                    case "Weapon":
                        string weaponId = dataElement.Attribute("id")?.Value;
                        valid = dataElement.Attribute("valid")?.Value;

                        if (string.IsNullOrEmpty(weaponId))
                            continue;

                        if (bool.TryParse(valid, out bool weaponValidresult))
                        {
                            heroOverride.IsValidWeaponByWeaponId.Add(weaponId, weaponValidresult);

                            if (!weaponValidresult)
                                continue;
                        }

                        overrideElement = dataElement.Element("Override");
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
                        overrideElement = dataElement.Element("Override");
                        if (overrideElement != null)
                            portraitOverride.SetOverride(cHeroId, overrideElement, heroOverride.PropertyPortraitOverrideMethodByCHeroId);
                        break;
                }
            }

            if (!HeroOverridesByCHeroId.ContainsKey(cHeroId))
                HeroOverridesByCHeroId.Add(cHeroId, heroOverride);
        }

        private void AddHeroUnits(string elementId, XElement element, HeroOverride heroOverride)
        {
            heroOverride.HeroUnits.Add(elementId);
            SetHeroOverrides(element);
        }
    }
}
