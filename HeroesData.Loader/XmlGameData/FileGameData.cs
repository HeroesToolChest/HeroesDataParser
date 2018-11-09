using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace HeroesData.Loader.XmlGameData
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

                if (!heroFolder.ToLower().Contains("data"))
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
                string xmlHeroPath = Path.Combine(NewHeroesFolderPath, heroFolder, "base.stormdata", GameDataStringName, $"{heroName}{DataStringName}.xml");
                string xmlHeroNamePath = Path.Combine(NewHeroesFolderPath, heroFolder, "base.stormdata", GameDataStringName, $"{heroName}.xml");

                string xmlHeroDataPath = Path.Combine(NewHeroesFolderPath, heroFolder, "base.stormdata", GameDataStringName, $"{HeroDataStringName}.xml");

                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
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
                }
                else // linux file names are case sensitive
                {
                    bool found = false;
                    string xmlDirectoryPath = Path.Combine(NewHeroesFolderPath, heroFolder, "base.stormdata", GameDataStringName);
                    foreach (string filePath in Directory.EnumerateFiles(xmlDirectoryPath))
                    {
                        if (Path.GetFileName(filePath).ToLower() == $"{heroName}{DataStringName}.xml".ToLower())
                        {
                            XmlGameData.Root.Add(XDocument.Load(filePath).Root.Elements());
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        foreach (string filePath in Directory.EnumerateFiles(xmlDirectoryPath))
                        {
                            if (Path.GetFileName(filePath).ToLower() == $"{heroName}.xml".ToLower())
                            {
                                XmlGameData.Root.Add(XDocument.Load(filePath).Root.Elements());
                                break;
                            }
                        }
                    }
                }

                if (File.Exists(xmlHeroDataPath))
                {
                    XmlGameData.Root.Add(XDocument.Load(xmlHeroDataPath).Root.Elements());
                    XmlFileCount++;
                }
            }
        }

        protected override void LoadHeroesMapMods()
        {
            foreach (string mapModsFolderPath in Directory.GetDirectories(HeroesMapModsFolderPath))
            {
                string mapFolderName = Path.GetFileName(mapModsFolderPath);
                string xmlMapGameDataPath = Path.Combine(HeroesMapModsFolderPath, mapFolderName, "base.stormdata", GameDataStringName);

                foreach (string xmlFilePath in Directory.GetFiles(xmlMapGameDataPath))
                {
                    if (Path.GetExtension(xmlFilePath) == ".xml")
                    {
                        XmlGameData.Root.Add(XDocument.Load(xmlFilePath).Root.Elements());
                        XmlFileCount++;
                    }
                }
            }
        }
    }
}
