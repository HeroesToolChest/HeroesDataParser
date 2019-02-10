using HeroesData.Loader.XmlGameData;
using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public abstract class OverrideLoaderBase<T>
        where T : class, IDataOverride
    {
        private readonly SortedDictionary<int, string> OverrideFileNamesByBuild = new SortedDictionary<int, string>(); // includes path

        public OverrideLoaderBase(GameData gameData, int? hotsBuild)
        {
            GameData = gameData;
            HotsBuild = hotsBuild;
        }

        public string LoadedOverrideFileName { get; private set; }

        protected Dictionary<string, T> DataOverridesById { get; } = new Dictionary<string, T>();
        protected string DataOverridesDirectoryPath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "dataoverrides");

        protected GameData GameData { get; }
        protected int? HotsBuild { get; }

        protected virtual string OverrideFileName { get; } = "overrides.xml";
        protected abstract string OverrideElementName { get; }

        public void Load()
        {
            LoadBuildNumberOverrideFiles();

            XDocument dataOverrideDocument = LoadOverrideFile();
            IEnumerable<XElement> dataElements = dataOverrideDocument.Root.Elements(OverrideElementName).Where(x => x.Attribute("id") != null);

            foreach (XElement dataElement in dataElements)
            {
                SetOverride(dataElement);
            }
        }

        /// <summary>
        /// Gets an override object from a given id. Returns null if none found.
        /// </summary>
        /// <param name="id">The id the override object.</param>
        /// <returns></returns>
        public T GetOverride(string id)
        {
            if (DataOverridesById.TryGetValue(id, out T overrideData))
                return overrideData;
            else
                return null;
        }

        protected abstract void SetOverride(XElement element);

        private XDocument LoadOverrideFile()
        {
            try
            {
                if (HotsBuild.HasValue)
                {
                    if (OverrideFileNamesByBuild.Count > 0)
                    {
                        // exact build number override file
                        if (OverrideFileNamesByBuild.TryGetValue(HotsBuild.Value, out string filePath))
                        {
                            LoadedOverrideFileName = filePath;
                        }
                        else if (HotsBuild.Value <= OverrideFileNamesByBuild.Keys.Min()) // load lowest
                        {
                            LoadedOverrideFileName = OverrideFileNamesByBuild[OverrideFileNamesByBuild.Keys.Min()];
                        }
                        else if (HotsBuild.Value >= OverrideFileNamesByBuild.Keys.Max()) // load the default
                        {
                            LoadedOverrideFileName = Path.Combine(DataOverridesDirectoryPath, OverrideFileName);
                        }
                        else // load next lowest
                        {
                            LoadedOverrideFileName = OverrideFileNamesByBuild.Aggregate((x, y) => Math.Abs(x.Key - HotsBuild.Value) < Math.Abs(y.Key - HotsBuild.Value) ? x : y).Value;
                        }

                        return XDocument.Load(LoadedOverrideFileName);
                    }
                }

                // default load
                if (File.Exists(Path.Combine(DataOverridesDirectoryPath, OverrideFileName)))
                {
                    LoadedOverrideFileName = OverrideFileName;

                    return XDocument.Load(Path.Combine(DataOverridesDirectoryPath, OverrideFileName));
                }

                return null;
            }
            catch (FileNotFoundException)
            {
                if (HotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {OverrideFileName} or {Path.GetFileNameWithoutExtension(OverrideFileName)}_{HotsBuild}.xml at {DataOverridesDirectoryPath}");
                else
                    throw new FileNotFoundException($"File not found: {OverrideFileName} at {DataOverridesDirectoryPath}");
            }
        }

        private void LoadBuildNumberOverrideFiles()
        {
            // get all _<number>.xml files
            foreach (string filePath in Directory.EnumerateFiles(DataOverridesDirectoryPath, $"{Path.GetFileNameWithoutExtension(OverrideFileName)}_*.xml"))
            {
                if (int.TryParse(Path.GetFileNameWithoutExtension(filePath).Split('_').LastOrDefault(), out int buildNumber))
                {
                    OverrideFileNamesByBuild.Add(buildNumber, filePath);
                }
            }
        }
    }
}
