using Heroes.Models;
using System.Collections.Generic;

namespace HeroesData.Parser
{
    public interface IParsableXmlData<T>
        where T : IExtractable
    {
        /// <summary>
        /// Returns the total number of parseable items.
        /// </summary>
        /// <returns></returns>
        int TotalParseable { get; }

        /// <summary>
        /// Gets or sets the hots build number.
        /// </summary>
        int? HotsBuild { get; set; }

        /// <summary>
        /// Gets or sets the localization.
        /// </summary>
        Localization Localization { get; set; }

        /// <summary>
        /// Returns a collection of parsed data.
        /// </summary>
        /// <param name="localization"></param>
        /// <remarks>Parsing is yielded.</remarks>
        /// <returns></returns>
        IEnumerable<T> Parse(Localization localization);
    }
}
