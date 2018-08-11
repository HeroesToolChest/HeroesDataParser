using HeroesData.Parser.GameStrings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesData
{
    public static class GameStringParse
    {
        public static (ConcurrentDictionary<string, string> parsed, ConcurrentDictionary<string, string> invalid) Parse(SortedDictionary<string, string> gameStringData, GameStringParser gameStringParser, string message, int maxDegreeOfParallelism)
        {
            ConcurrentDictionary<string, string> parsed = new ConcurrentDictionary<string, string>();
            ConcurrentDictionary<string, string> invalid = new ConcurrentDictionary<string, string>();

            int currentCount = 0;
            Parallel.ForEach(gameStringData, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, tooltip =>
            {
                try
                {
                    if (gameStringParser.TryParseRawTooltip(tooltip.Key, tooltip.Value, out string parsedTooltip))
                        parsed.GetOrAdd(tooltip.Key, parsedTooltip);
                    else
                        invalid.GetOrAdd(tooltip.Key, tooltip.Value);
                }
                finally
                {
                    Interlocked.Increment(ref currentCount);

                    Console.Write($"\r{currentCount,6} / {gameStringData.Count} {message}");
                }
            });

            Console.WriteLine();

            return (parsed, invalid);
        }
    }
}
