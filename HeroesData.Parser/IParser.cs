using Heroes.Models;
using System.Collections.Generic;

namespace HeroesData.Parser
{
    public interface IParser<T, TU>
        where T : IExtractable
    {
        /// <summary>
        /// Returns a collection of all the parsable ids. Allows multiple ids.
        /// </summary>
        /// <returns></returns>
        HashSet<string[]> Items { get; }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        int? HotsBuild { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        Localization Localization { get; set; }

        /// <summary>
        /// Returns the parsed game data from the given ids. Multiple ids may be used to identity one item.
        /// </summary>
        /// <param name="id">The ids of the item to parse.</param>
        /// <returns></returns>
        T Parse(params string[] ids);

        /// <summary>
        /// Gets a new instance of the object.
        /// </summary>
        /// <returns></returns>
        TU GetInstance();
    }
}
