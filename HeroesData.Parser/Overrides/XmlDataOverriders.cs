using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Overrides
{
    public class XmlDataOverriders
    {
        private readonly string _appPath;
        private readonly GameData _gameData;
        private readonly int? _hotsBuild;
        private readonly string _overrideFileNameSuffix;

        private readonly Dictionary<Type, IOverrideLoader> _overrides = new Dictionary<Type, IOverrideLoader>();

        private XmlDataOverriders(string appPath, GameData gameData, string overrideFileNameSuffix)
            : this(appPath, gameData, null, overrideFileNameSuffix)
        {
        }

        private XmlDataOverriders(string appPath, GameData gameData, int? hotsBuild, string overrideFileNameSuffix)
        {
            _appPath = appPath;
            _gameData = gameData;
            _hotsBuild = hotsBuild;
            _overrideFileNameSuffix = overrideFileNameSuffix;

            Initialize();
        }

        /// <summary>
        /// Gets a collection of loaded override file names.
        /// </summary>
        public IEnumerable<string> LoadedFileNames => _overrides.Values.Select(x => x.LoadedOverrideFileName).ToList();

        /// <summary>
        /// Gets the amount of overriders that have been set.
        /// </summary>
        public int Count => _overrides.Count;

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
            if (_overrides.TryGetValue(type, out IOverrideLoader? overrides))
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
            _overrides.Add(typeof(HeroDataParser), new HeroOverrideLoader(_appPath, _hotsBuild));
            _overrides.Add(typeof(UnitParser), new UnitOverrideLoader(_appPath, _hotsBuild));
            _overrides.Add(typeof(MatchAwardParser), new MatchAwardOverrideLoader(_appPath, _hotsBuild));
        }

        private void LoadOverrides()
        {
            foreach (var overrider in _overrides)
            {
                overrider.Value.Load(_overrideFileNameSuffix);
            }
        }
    }
}
