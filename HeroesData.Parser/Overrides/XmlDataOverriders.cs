using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Overrides
{
    public class XmlDataOverriders
    {
        private readonly string AppPath;
        private readonly GameData GameData;
        private readonly int? HotsBuild;
        private readonly string OverrideFileNameSuffix;

        private readonly Dictionary<Type, IOverrideLoader> Overrides = new Dictionary<Type, IOverrideLoader>();

        private XmlDataOverriders(string appPath, GameData gameData, string overrideFileNameSuffix)
            : this(appPath, gameData, null, overrideFileNameSuffix)
        {
        }

        private XmlDataOverriders(string appPath, GameData gameData, int? hotsBuild, string overrideFileNameSuffix)
        {
            AppPath = appPath;
            GameData = gameData;
            HotsBuild = hotsBuild;
            OverrideFileNameSuffix = overrideFileNameSuffix;

            Initialize();
        }

        /// <summary>
        /// Returns a collection of loaded override file names.
        /// </summary>
        public IEnumerable<string> LoadedFileNames => Overrides.Values.Select(x => x.LoadedOverrideFileName).ToList();

        /// <summary>
        /// Returns the amount of overriders that have been set.
        /// </summary>
        public int Count => Overrides.Count;

        /// <summary>
        /// Loads all the override data.
        /// </summary>
        /// <param name="appPath">The path of the app host.</param>
        /// <param name="gameData">GameData.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(string appPath, GameData gameData)
        {
            return new XmlDataOverriders(appPath, gameData, string.Empty);
        }

        /// <summary>
        /// Sets and loads all the override data.
        /// </summary>
        /// <param name="appPath">The path of the app host.</param>
        /// <param name="gameData">GameData.</param>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(string appPath, GameData gameData, string overrideFileNameSuffix)
        {
            return new XmlDataOverriders(appPath, gameData, overrideFileNameSuffix);
        }

        /// <summary>
        /// Sets and loads all the override data.
        /// </summary>
        /// <param name="appPath">The path of the app host.</param>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(string appPath, GameData gameData, int? hotsBuild)
        {
            return new XmlDataOverriders(appPath, gameData, hotsBuild, string.Empty);
        }

        /// <summary>
        /// Loads all the override data.
        /// </summary>
        /// <param name="appPath">The path of the app host.</param>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(string appPath, GameData gameData, int? hotsBuild, string overrideFileNameSuffix)
        {
            return new XmlDataOverriders(appPath, gameData, hotsBuild, overrideFileNameSuffix);
        }

        /// <summary>
        /// Returns an override loader from a given parser type.
        /// </summary>
        /// <param name="type">The type of parser.</param>
        /// <returns></returns>
        public IOverrideLoader? GetOverrider(Type type)
        {
            if (Overrides.TryGetValue(type, out IOverrideLoader? overrides))
                return overrides;
            else
                return null;
        }

        private void Initialize()
        {
            SetDataOverrides();
            LoadOverrides();
        }

        private void SetDataOverrides()
        {
            Overrides.Add(typeof(HeroDataParser), new HeroOverrideLoader(AppPath, HotsBuild));
            Overrides.Add(typeof(UnitParser), new UnitOverrideLoader(AppPath, HotsBuild));
            Overrides.Add(typeof(MatchAwardParser), new MatchAwardOverrideLoader(AppPath, HotsBuild));
        }

        private void LoadOverrides()
        {
            foreach (var overrider in Overrides)
            {
                overrider.Value.Load(OverrideFileNameSuffix);
            }
        }
    }
}
