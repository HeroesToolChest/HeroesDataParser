using Heroes.Icons.FileWriter.Writer;
using Heroes.Icons.Parser.Models;
using System.Collections.Generic;

namespace Heroes.Icons.FileWriter
{
    public class FileOutput
    {
        private FileOutput(List<Hero> heroes)
        {
            FileConfiguration fileConfiguration = FileConfiguration.Load();
            XmlWriter.CreateOutput(fileConfiguration.XmlFileSettings, heroes);
        }

        public static FileOutput CreateOutput(List<Hero> heroes)
        {
            return new FileOutput(heroes);
        }
    }
}
