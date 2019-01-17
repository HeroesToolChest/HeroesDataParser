using System.Collections.Generic;

namespace HeroesData.FileWriter.Writer
{
    internal interface IWriter<T>
    {
        void CreateOutput(IEnumerable<T> items);
    }
}
