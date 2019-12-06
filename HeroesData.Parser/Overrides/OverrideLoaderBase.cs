using HeroesData.Parser.Overrides.DataOverrides;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace HeroesData.Parser.Overrides
{
    public abstract class OverrideLoaderBase<T>
        where T : class, IDataOverride
    {
        private readonly string AppPath;
        private readonly int? HotsBuild;
        private readonly SortedDictionary<int, string> OverrideFileNamesByBuild = new SortedDictionary<int, string>(); // includes path

        public OverrideLoaderBase(int? hotsBuild)
            : this(string.Empty, hotsBuild)
        {
        }

        public OverrideLoaderBase(string appPath, int? hotsBuild)
        {
            AppPath = appPath;
            HotsBuild = hotsBuild;
        }

        /// <summary>
        /// The loaded override file name (includes path).
        /// </summary>
        public string LoadedOverrideFileName { get; private set; } = string.Empty;

        /// <summary>
        /// Returns the amount of data overrides loaded from override file.
        /// </summary>
        public int Count => DataOverridesById.Count;

        protected Dictionary<string, T> DataOverridesById { get; private set; } = new Dictionary<string, T>();
        protected string DataOverridesDirectoryPath => Path.Combine(AppPath, "dataoverrides");
        protected virtual string OverrideFileName { get; private set; } = "overrides.xml";
        protected abstract string OverrideElementName { get; }

        /// <summary>
        /// Loads the override file.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        public void Load()
        {
            DataOverridesById.Clear();

            LoadBuildNumberOverrideFiles();

            XDocument dataOverrideDocument = LoadOverrideFile();
            IEnumerable<XElement> dataElements = dataOverrideDocument.Root.Elements(OverrideElementName).Where(x => x.Attribute("id") != null);

            foreach (XElement dataElement in dataElements)
            {
                SetOverride(dataElement);
            }
        }

        /// <summary>
        /// Loads the override file.
        /// </summary>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        /// <exception cref="FileNotFoundException"></exception>
        public void Load(string overrideFileNameSuffix)
        {
            if (!string.IsNullOrEmpty(overrideFileNameSuffix))
            {
                if (!Path.HasExtension(overrideFileNameSuffix))
                    overrideFileNameSuffix += ".xml";

                OverrideFileName = overrideFileNameSuffix;
            }

            Load();
        }

        /// <summary>
        /// Gets an override object from a given id. Returns null if none found.
        /// </summary>
        /// <param name="id">The id the override object.</param>
        /// <returns></returns>
        public T? GetOverride(string id)
        {
            if (DataOverridesById == null)
                throw new NullReferenceException("The Load() method needs to be called before this method can be used.");

            if (id.Contains(".stormmod"))
                id = id.Replace(".stormmod", string.Empty);

            if (DataOverridesById.TryGetValue(id, out T? overrideData))
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
                        if (OverrideFileNamesByBuild.TryGetValue(HotsBuild.Value, out string? filePath))
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
                            LoadedOverrideFileName = OverrideFileNamesByBuild.Aggregate((x, y) => Math.Abs(x.Key - HotsBuild.Value) <= Math.Abs(y.Key - HotsBuild.Value) ? x : y).Value;
                        }

                        return XDocument.Load(LoadedOverrideFileName);
                    }
                }

                // default load
                if (File.Exists(Path.Combine(DataOverridesDirectoryPath, OverrideFileName)))
                {
                    LoadedOverrideFileName = Path.Combine(DataOverridesDirectoryPath, OverrideFileName);

                    return XDocument.Load(Path.Combine(DataOverridesDirectoryPath, OverrideFileName));
                }

                throw new FileNotFoundException();
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
