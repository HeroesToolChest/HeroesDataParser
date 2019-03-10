using System.Collections.Generic;
using System.Threading.Tasks;

namespace HeroesData.FileWriter.Writers
{
    internal interface IWriter<T>
    {
        Task CreateOutputAsync(IEnumerable<T> items);
    }
}
