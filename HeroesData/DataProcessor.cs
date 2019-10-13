using Heroes.Models;
using System;
using System.Collections.Generic;

namespace HeroesData
{
    internal class DataProcessor
    {
        public bool IsEnabled { get; set; }
        public string Name { get; set; } = string.Empty;

        public Func<Localization, IEnumerable<IExtractable?>>? Parse { get; set; }
        public Action<Localization>? Validate { get; set; }
        public Action<IEnumerable<IExtractable>>? Extract { get; set; }

        public IEnumerable<IExtractable?>? ParsedItems { get; set; }
    }
}
