using System.IO;
using System.Linq;

namespace Heroes.Icons.FileWriter.Writer
{
    internal class Writer
    {
        protected Writer()
        {
            Directory.CreateDirectory(OutputFolder);
        }

        protected string OutputFolder => "Output";

        protected string StripInvalidChars(string text)
        {
            return new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }
    }
}
