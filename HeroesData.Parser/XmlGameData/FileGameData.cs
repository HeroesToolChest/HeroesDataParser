using System.IO;
using System.Xml.Linq;

namespace HeroesData.Parser.XmlGameData
{
    public class FileGameData : GameData
    {
        public FileGameData(string modsFolderPath)
            : base(modsFolderPath)
        {
            Initialize();
        }

        public FileGameData(string modsFolderPath, int? hotsBuild)
            : base(modsFolderPath, hotsBuild)
        {
            Initialize();
        }

        protected override void LoadCoreStormMod()
        {
            foreach (string file in Directory.GetFiles(CoreStormModFolderPath))
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;

                if (XmlGameData.LastNode == null)
                {
                    XmlGameData = XDocument.Load(file);
                    XmlFileCount++;
                }
                else
                {
                    XmlGameData.Root.Add(XDocument.Load(file).Root.Elements());
                    XmlFileCount++;
                }
            }
        }

        protected override void LoadHeroesDataStormMod()
        {
            foreach (string file in Directory.GetFiles(HeroesdataStormModFolderPath))
            {
                if (Path.GetExtension(file) != ".xml")
                    continue;

                XmlGameData.Root.Add(XDocument.Load(file).Root.Elements());
                XmlFileCount++;
            }
        }

        protected override void LoadOldHeroes()
        {
            foreach (string heroFolderPath in Directory.GetDirectories(OldHeroesFolderPath))
            {
                string heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("Data"))
                    continue;

                string xmlHeroPath = Path.Combine(OldHeroesFolderPath, heroFolder, heroFolder + ".xml");
                XmlGameData.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                XmlFileCount++;
            }
        }

        protected override void LoadNewHeroes()
        {
            foreach (string heroFolderPath in Directory.GetDirectories(NewHeroesFolderPath))
            {
                string heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("stormmod") || heroFolder == "herointeractions.stormmod")
                    continue;

                string heroName = heroFolder.Split('.')[0];
                string xmlHeroPath = Path.Combine(NewHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}Data.xml");
                string xmlHeroNamePath = Path.Combine(NewHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}.xml");
                string xmlHeroDataPath = Path.Combine(NewHeroesFolderPath, heroFolder, @"base.stormdata\GameData\HeroData.xml");

                if (File.Exists(xmlHeroPath))
                {
                    XmlGameData.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                    XmlFileCount++;
                }
                else
                {
                    XmlGameData.Root.Add(XDocument.Load(xmlHeroNamePath).Root.Elements());
                    XmlFileCount++;
                }

                if (File.Exists(xmlHeroDataPath))
                {
                    XmlGameData.Root.Add(XDocument.Load(xmlHeroDataPath).Root.Elements());
                    XmlFileCount++;
                }
            }
        }
    }
}
