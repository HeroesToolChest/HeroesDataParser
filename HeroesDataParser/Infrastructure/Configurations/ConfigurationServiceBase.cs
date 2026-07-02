namespace HeroesDataParser.Infrastructure.Configurations;

public abstract class ConfigurationServiceBase
{
    public ConfigurationServiceBase(RootOptions options)
    {
        Options = options;
    }

    protected RootOptions Options { get; }

    public abstract void Load();

    protected abstract void LoadFiles();

    protected abstract void ProcessFiles();

    protected string? GetSelectedFilePath(SortedDictionary<int, string> relativeFilePathsByBuild)
    {
        string? selectedFilePath = null;

        // check if a build number was set
        if (Options.BuildNumber.HasValue)
        {
            // are there any files loaded
            if (relativeFilePathsByBuild.Count > 0)
            {
                // exact match
                if (relativeFilePathsByBuild.TryGetValue(Options.BuildNumber.Value, out string? filePath))
                {
                    selectedFilePath = filePath;
                }
                else if (Options.BuildNumber.Value <= relativeFilePathsByBuild.Keys.Min())
                {
                    // lowest build number
                    selectedFilePath = relativeFilePathsByBuild.First().Value;
                }
                else
                {
                    // load next lowest
                    int index = relativeFilePathsByBuild.Keys.ToList().BinarySearch(Options.BuildNumber.Value);

                    int closestLowerIndex = ~index - 1;
                    if (closestLowerIndex >= 0)
                    {
                        selectedFilePath = relativeFilePathsByBuild.ElementAt(closestLowerIndex).Value;
                    }
                }
            }
        }
        else if (relativeFilePathsByBuild.Count > 0)
        {
            // default
            selectedFilePath = relativeFilePathsByBuild.Last().Value;
        }

        return selectedFilePath;
    }
}
