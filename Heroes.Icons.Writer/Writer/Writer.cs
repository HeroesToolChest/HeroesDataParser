using System.IO;

namespace Heroes.Icons.FileWriter.Writer
{
    internal class Writer
    {
        protected Writer()
        {
            Directory.CreateDirectory(OutputFolder);
        }

        protected string OutputFolder => "Output";
    }
}
