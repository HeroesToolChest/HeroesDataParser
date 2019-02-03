using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData.Data
{
    internal class DataParser
    {
        public bool IsEnabled { get; set; }

        public Func<Localization, IEnumerable<IExtractable>> Parse { get; set; }
        public Action<Localization> Validate { get; set; }
    }
}
