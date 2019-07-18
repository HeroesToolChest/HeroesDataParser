using System;
using System.Collections.Generic;
using System.IO;

namespace HeroesData.FileWriter.Writers
{
    internal class GameStringWriter
    {
        public Dictionary<string, string> GameStrings { get; } = new Dictionary<string, string>();

        public void Update(string gamestringDirectory, string gamestringTextFileName)
        {
            string filePath = Path.Combine(gamestringDirectory, gamestringTextFileName);

            // load current existing
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    while (!reader.EndOfStream)
                    {
                        string[] line = reader.ReadLine().Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (line.Length == 2)
                            GameStrings[line[0]] = line[1];
                    }
                }
            }
        }

        public void Write(string gamestringDirectory, string gamestringTextFileName)
        {
            List<string> list = new List<string>();

            foreach (var gamestring in GameStrings)
            {
                list.Add($"{gamestring.Key}={gamestring.Value}");
            }

            list.Sort();

            using (StreamWriter writer = new StreamWriter(Path.Combine(gamestringDirectory, gamestringTextFileName), false))
            {
                foreach (string item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }

        public void AddUnitName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/name/{key}"] = value;
        }

        public void AddUnitDifficulty(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/difficulty/{key}"] = value;
        }

        public void AddUnitType(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/type/{key}"] = value;
        }

        public void AddUnitRole(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/role/{key}"] = value;
        }

        public void AddUnitExpandedRole(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/expandedrole/{key}"] = value;
        }

        public void AddUnitDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/description/{key}"] = value;
        }

        public void AddUnitDamageType(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/damagetype/{key}"] = value;
        }

        public void AddHeroTitle(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/title/{key}"] = value;
        }

        public void AddHeroSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/searchtext/{key}"] = value;
        }

        public void AddUnitLifeType(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/lifetype/{key}"] = value;
        }

        public void AddUnitEnergyType(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/energytype/{key}"] = value;
        }

        public void AddUnitShieldType(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"unit/shieldtype/{key}"] = value;
        }

        public void AddAbilityTalentName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/name/{key}"] = value;
        }

        public void AddAbilityTalentLifeTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/life/{key}"] = value;
        }

        public void AddAbilityTalentEnergyTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/energy/{key}"] = value;
        }

        public void AddAbilityTalentCooldownTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/cooldown/{key}"] = value;
        }

        public void AddAbilityTalentShortTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/short/{key}"] = value;
        }

        public void AddAbilityTalentFullTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"abiltalent/full/{key}"] = value;
        }

        public void AddMatchAwardName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"award/name/{key}"] = value;
        }

        public void AddMatchAwardDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"award/description/{key}"] = value;
        }

        public void AddHeroSkinName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"heroskin/name/{key}"] = value;
        }

        public void AddHeroSkinSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"heroskin/sortname/{key}"] = value;
        }

        public void AddHeroSkinInfo(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"heroskin/info/{key}"] = value;
        }

        public void AddHeroSkinSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"heroskin/searchtext/{key}"] = value;
        }

        public void AddMountName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"mount/name/{key}"] = value;
        }

        public void AddMountSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"mount/sortname/{key}"] = value;
        }

        public void AddMountInfo(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"mount/info/{key}"] = value;
        }

        public void AddMountSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"mount/searchtext/{key}"] = value;
        }

        public void AddBannerName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"banner/name/{key}"] = value;
        }

        public void AddBannerSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"banner/sortname/{key}"] = value;
        }

        public void AddBannerDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"banner/description/{key}"] = value;
        }

        public void AddSprayName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"spray/name/{key}"] = value;
        }

        public void AddSpraySortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"spray/sortname/{key}"] = value;
        }

        public void AddSprayDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"spray/description/{key}"] = value;
        }

        public void AddSpraySearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"spray/searchtext/{key}"] = value;
        }

        public void AddAnnouncerName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"announcer/name/{key}"] = value;
        }

        public void AddAnnouncerSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"announcer/sortname/{key}"] = value;
        }

        public void AddAnnouncerDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"announcer/description/{key}"] = value;
        }

        public void AddPortraitName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"portrait/name/{key}"] = value;
        }

        public void AddPortraitSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"portrait/sortname/{key}"] = value;
        }

        public void AddVoiceLineName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"voiceline/name/{key}"] = value;
        }

        public void AddVoiceLineSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"voiceline/sortname/{key}"] = value;
        }

        public void AddVoiceLineDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"voiceline/description/{key}"] = value;
        }

        public void AddEmoticonName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticon/name/{key}"] = value;
        }

        public void AddEmoticonAlias(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticon/alias/{key}"] = value;
        }

        public void AddEmoticonSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticon/searchtext/{key}"] = value;
        }

        public void AddEmoticonDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticon/description/{key}"] = value;
        }

        public void AddEmoticonPackName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticonpack/name/{key}"] = value;
        }

        public void AddEmoticonPackSortName(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticonpack/sortname/{key}"] = value;
        }

        public void AddEmoticonPackDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                return;

            GameStrings[$"emoticonpack/description/{key}"] = value;
        }
    }
}
