using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.GameStrings;
using System;

namespace HeroesData.Parser
{
    public static class GameDataExtensions
    {
        private static Lazy<GameStringParser> LazyGameStringParser = new Lazy<GameStringParser>();

        public static string GetParsedGameString(this GameData gameData, string id)
        {
            if (!LazyGameStringParser.IsValueCreated)
                LazyGameStringParser = new Lazy<GameStringParser>(() => new GameStringParser(gameData, gameData.HotsBuild));

            if (LazyGameStringParser.Value.TryParseRawTooltip(id, gameData.GetGameString(id), out string parsedTooltip))
                return parsedTooltip;
            else
                return null;
        }

        public static bool TryGetParsedGameString(this GameData gameData, string id, out string value)
        {
            if (!LazyGameStringParser.IsValueCreated)
                LazyGameStringParser = new Lazy<GameStringParser>(() => new GameStringParser(gameData, gameData.HotsBuild));

            if (LazyGameStringParser.Value.TryParseRawTooltip(id, gameData.GetGameString(id), out value))
                return true;
            else
                return false;
        }
    }
}
