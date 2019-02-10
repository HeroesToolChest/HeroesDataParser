namespace HeroesData.Parser.Overrides
{
    public interface IOverrideLoader
    {
        string LoadedOverrideFileName { get; }

        void Load();
    }
}
