namespace HeroesData.Parser.Overrides
{
    public interface IOverrideLoader
    {
        string LoadedOverrideFileName { get; }

        /// <summary>
        /// Loads the override file.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        void Load();

        /// <summary>
        /// Loads the override file.
        /// </summary>
        /// <param name="overrideFileNameSuffix">Sets the suffix of the override file name to load. The suffix is the part after the first hypen. It does not have to include the file extension.</param>
        void Load(string overrideFileNameSuffix);
    }
}
