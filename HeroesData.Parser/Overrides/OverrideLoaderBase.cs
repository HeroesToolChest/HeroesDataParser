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
        private readonly string _appPath;
        private readonly int? _hotsBuild;
        private readonly SortedDictionary<int, string> _overrideFileNamesByBuild = new SortedDictionary<int, string>(); // includes path

        public OverrideLoaderBase(int? hotsBuild)
            : this(string.Empty, hotsBuild)
        {
        }

        public OverrideLoaderBase(string appPath, int? hotsBuild)
        {
            _appPath = appPath;
            _hotsBuild = hotsBuild;
        }

        /// <summary>
        /// Gets the loaded override file name (includes path).
        /// </summary>
        public string LoadedOverrideFileName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the amount of data overrides loaded from override file.
        /// </summary>
        public int Count => DataOverridesById.Count;

        protected Dictionary<string, T> DataOverridesById { get; private set; } = new Dictionary<string, T>();
        protected string DataOverridesDirectoryPath => Path.Combine(_appPath, "dataoverrides");
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
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            if (DataOverridesById == null)
                throw new NullReferenceException("The Load() method needs to be called before this method can be used.");

            if (id.Contains(".stormmod", StringComparison.OrdinalIgnoreCase))
                id = id.Replace(".stormmod", string.Empty, StringComparison.OrdinalIgnoreCase);

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
                if (_hotsBuild.HasValue)
                {
                    if (_overrideFileNamesByBuild.Count > 0)
                    {
                        // exact build number override file
                        if (_overrideFileNamesByBuild.TryGetValue(_hotsBuild.Value, out string? filePath))
                        {
                            LoadedOverrideFileName = filePath;
                        }
                        else if (_hotsBuild.Value <= _overrideFileNamesByBuild.Keys.Min()) // load lowest
                        {
                            LoadedOverrideFileName = _overrideFileNamesByBuild[_overrideFileNamesByBuild.Keys.Min()];
                        }
                        else if (_hotsBuild.Value >= _overrideFileNamesByBuild.Keys.Max()) // load the default
                        {
                            LoadedOverrideFileName = Path.Combine(DataOverridesDirectoryPath, OverrideFileName);
                        }
                        else // load next lowest
                        {
                            LoadedOverrideFileName = _overrideFileNamesByBuild.Aggregate((x, y) => Math.Abs(x.Key - _hotsBuild.Value) <= Math.Abs(y.Key - _hotsBuild.Value) ? x : y).Value;
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
                if (_hotsBuild.HasValue)
                    throw new FileNotFoundException($"File not found: {OverrideFileName} or {Path.GetFileNameWithoutExtension(OverrideFileName)}_{_hotsBuild}.xml at {DataOverridesDirectoryPath}");
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
                    _overrideFileNamesByBuild.Add(buildNumber, filePath);
                }
            }
        }
    }
}
