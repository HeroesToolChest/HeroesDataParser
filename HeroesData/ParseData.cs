using Heroes.Models;
using HeroesData.Parser.HeroData;
using HeroesData.Parser.MatchAwardData;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using static HeroesData.App;

namespace HeroesData
{
    internal static class ParseData
    {
        public static IEnumerable<Hero> ParseHeroes(Localization localization)
        {
            Stopwatch time = new Stopwatch();
            var parsedHeroes = new ConcurrentDictionary<string, Hero>();
            var failedParsedHeroes = new List<(string CHeroId, Exception Exception)>();
            int currentCount = 0;

            Console.WriteLine($"Parsing hero data...");

            time.Start();

            // get the base hero first
            HeroParser heroParserBase = new HeroParser(GameData, DefaultData, OverrideData)
            {
                HotsBuild = HotsBuild,
                Localization = localization,
            };

            // parse the base hero and add it to parsedHeroes
            Hero baseHeroData = heroParserBase.ParseBaseHero();
            parsedHeroes.GetOrAdd(baseHeroData.CHeroId, baseHeroData);
            SortedSet<string> heroNames = heroParserBase.GetCHeroNames();

            // parse all the heroes
            Console.Write($"\r{currentCount,6} / {heroNames.Count} total heroes");
            Parallel.ForEach(heroNames, new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, cHeroId =>
            {
                try
                {
                    HeroParser heroParser = new HeroParser(GameData, DefaultData, OverrideData)
                    {
                        HotsBuild = HotsBuild,
                        Localization = localization,
                        StormHeroBase = baseHeroData,
                    };

                    parsedHeroes.GetOrAdd(cHeroId, heroParser.Parse(cHeroId));
                }
                catch (Exception ex)
                {
                    failedParsedHeroes.Add((cHeroId, ex));
                }
                finally
                {
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} / {heroNames.Count} total heroes");
                }
            });

            time.Stop();

            Console.WriteLine();

            if (failedParsedHeroes.Count > 0)
            {
                foreach (var hero in failedParsedHeroes)
                {
                    WriteExceptionLog($"FailedHeroParsed_{hero.CHeroId}", hero.Exception);
                }
            }

            Console.WriteLine($"{parsedHeroes.Count - 1,6} successfully parsed heroes"); // minus 1 to account for base hero

            if (failedParsedHeroes.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var hero in failedParsedHeroes)
                {
                    Console.WriteLine($"{hero.CHeroId} - {hero.Exception.Message}");
                }

                Console.WriteLine($"{failedParsedHeroes.Count} failed to parse [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            Console.ResetColor();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return parsedHeroes.Values;
        }

        public static IEnumerable<MatchAward> ParseMatchAwards()
        {
            Stopwatch time = new Stopwatch();
            var parsedMatchAwards = new ConcurrentDictionary<string, MatchAward>();

            Console.WriteLine($"Parsing match award data...");

            time.Start();

            MatchAwardParser awardParser = new MatchAwardParser(GameData)
            {
                HotsBuild = HotsBuild,
            };

            int currentCount = 0;

            Console.Write($"\r{currentCount,6}");

            try
            {
                Parallel.ForEach(awardParser.Parse(), new ParallelOptions { MaxDegreeOfParallelism = MaxParallelism }, item =>
                {
                    parsedMatchAwards.GetOrAdd(item.ShortName, item);
                    Console.Write($"\r{Interlocked.Increment(ref currentCount),6} parsed awards");
                });
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                WriteExceptionLog($"{nameof(ParseMatchAwards)}", ex);

                Console.WriteLine();
                Console.WriteLine($"Failed to parse awards [Check logs for details]");
                Console.WriteLine();

                Console.ResetColor();
                Environment.Exit(1);
            }

            time.Stop();

            Console.WriteLine();
            Console.WriteLine($"Finished in {time.Elapsed.Seconds} seconds {time.Elapsed.Milliseconds} milliseconds");
            Console.WriteLine();

            return parsedMatchAwards.Values;
        }
    }
}
