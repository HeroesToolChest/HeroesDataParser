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
                        string[] line = reader.ReadLine().Split('=');
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
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/name/{key}"] = value;
        }

        public void AddUnitDifficulty(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/difficulty/{key}"] = value;
        }

        public void AddUnitType(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/type/{key}"] = value;
        }

        public void AddUnitRole(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/role/{key}"] = value;
        }

        public void AddUnitDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/description/{key}"] = value;
        }

        public void AddHeroTitle(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/title/{key}"] = value;
        }

        public void AddHeroSearchText(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"unit/searchtext/{key}"] = value;
        }

        public void AddAbilityTalentName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"abiltalent/name/{key}"] = value;
        }

        public void AddAbilityTalentLifeTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"tooltip/life/{key}"] = value;
        }

        public void AddAbilityTalentEnergyTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"tooltip/energy/{key}"] = value;
        }

        public void AddAbilityTalentCooldownTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"tooltip/cooldown/{key}"] = value;
        }

        public void AddAbilityTalentShortTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"tooltip/short/{key}"] = value;
        }

        public void AddAbilityTalentFullTooltip(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"tooltip/full/{key}"] = value;
        }

        public void AddMatchAwardName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"award/name/{key}"] = value;
        }

        public void AddMatchAwardDescription(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            GameStrings[$"award/description/{key}"] = value;
        }
    }
}
