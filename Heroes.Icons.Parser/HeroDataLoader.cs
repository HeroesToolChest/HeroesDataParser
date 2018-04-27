using System.IO;
using System.Xml.Linq;

namespace Heroes.Icons.Parser
{
    /// <summary>
    /// Loads all the .xml files into an XDocument
    /// </summary>
    public class HeroDataLoader
    {
        private string ModsFolderPath;

        public HeroDataLoader(string modsFolderPath)
        {
            ModsFolderPath = modsFolderPath;
        }

        public int XmlFileCount { get; set; } = 0;
        public XDocument XmlData { get; set; }

        /// <summary>
        /// Loads all the needed .xml files into a XDocument
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            string gameDataFolderPath = Path.Combine(ModsFolderPath, @"heroesdata.stormmod\base.stormdata\GameData\");
            string oldHeroesFolderPath = Path.Combine(gameDataFolderPath, "Heroes");
            string newHeroesFolderPath = Path.Combine(ModsFolderPath, "heromods");

            XDocument xDoc = new XDocument();

            // load all GameData xml files
            foreach (var gameDataFile in Directory.GetFiles(gameDataFolderPath))
            {
                if (Path.GetExtension(gameDataFile) != ".xml")
                    continue;

                if (xDoc.LastNode == null)
                {
                    xDoc = XDocument.Load(gameDataFile);
                    XmlFileCount++;
                }
                else
                {
                    xDoc.Root.Add(XDocument.Load(gameDataFile).Root.Elements());
                    XmlFileCount++;
                }
            }

            // old heroes
            foreach (var heroFolderPath in Directory.GetDirectories(oldHeroesFolderPath))
            {
                var heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("Data"))
                    continue;

                string xmlHeroPath = Path.Combine(oldHeroesFolderPath, heroFolder, heroFolder + ".xml");
                xDoc.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                XmlFileCount++;
            }

            // new heroes
            foreach (var heroFolderPath in Directory.GetDirectories(newHeroesFolderPath))
            {
                var heroFolder = Path.GetFileName(heroFolderPath);

                if (!heroFolder.Contains("stormmod") || heroFolder == "herointeractions.stormmod")
                    continue;

                string heroName = heroFolder.Split('.')[0];
                string xmlHeroPath = Path.Combine(newHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}Data.xml");
                string xmlHeroNamePath = Path.Combine(newHeroesFolderPath, heroFolder, $@"base.stormdata\GameData\{heroName}.xml");
                string xmlHeroDataPath = Path.Combine(newHeroesFolderPath, heroFolder, @"base.stormdata\GameData\HeroData.xml");

                if (File.Exists(xmlHeroPath))
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroPath).Root.Elements());
                    XmlFileCount++;
                }
                else
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroNamePath).Root.Elements());
                    XmlFileCount++;
                }

                if (File.Exists(xmlHeroDataPath))
                {
                    xDoc.Root.Add(XDocument.Load(xmlHeroDataPath).Root.Elements());
                    XmlFileCount++;
                }
            }

            XmlData = xDoc;
        }
    }
}
