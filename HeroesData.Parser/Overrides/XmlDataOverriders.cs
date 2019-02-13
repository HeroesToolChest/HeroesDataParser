using HeroesData.Loader.XmlGameData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroesData.Parser.Overrides
{
    public class XmlDataOverriders
    {
        private readonly GameData GameData;
        private readonly int? HotsBuild;
        private readonly string OverrideFileNameSuffix;

        private readonly Dictionary<Type, IOverrideLoader> Overrides = new Dictionary<Type, IOverrideLoader>();

        private XmlDataOverriders(GameData gameData, string overrideFileNameSuffix)
        {
            GameData = gameData;
            OverrideFileNameSuffix = overrideFileNameSuffix;

            Initialize();
        }

        private XmlDataOverriders(GameData gameData, int? hotsBuild, string overrideFileNameSuffix)
        {
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
        /// <param name="gameData">GameData.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(GameData gameData)
        {
            return new XmlDataOverriders(gameData, null);
        }

        /// <summary>
        /// Sets and loads all the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(GameData gameData, string overrideFileNameSuffix)
        {
            return new XmlDataOverriders(gameData, overrideFileNameSuffix);
        }

        /// <summary>
        /// Sets and loads all the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(GameData gameData, int? hotsBuild)
        {
            return new XmlDataOverriders(gameData, hotsBuild, null);
        }

        /// <summary>
        /// Loads all the override data.
        /// </summary>
        /// <param name="gameData">GameData.</param>
        /// <param name="hotsBuild">The override build version to load.</param>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        /// <returns></returns>
        public static XmlDataOverriders Load(GameData gameData, int? hotsBuild, string overrideFileNameSuffix)
        {
            return new XmlDataOverriders(gameData, hotsBuild, overrideFileNameSuffix);
        }

        /// <summary>
        /// Returns an override loader from a given parser type.
        /// </summary>
        /// <param name="type">The type of parser.</param>
        /// <returns></returns>
        public IOverrideLoader GetOverrider(Type type)
        {
            if (Overrides.TryGetValue(type, out IOverrideLoader overrides))
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
            Overrides.Add(typeof(HeroDataParser), new HeroOverrideLoader(GameData, HotsBuild));
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
