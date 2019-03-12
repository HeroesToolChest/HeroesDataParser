using System.Collections.Generic;

namespace HeroesData.FileWriter.Writers
{
    internal interface IWriter<T>
    {
        void CreateOutput(IEnumerable<T> items);
    }
}
