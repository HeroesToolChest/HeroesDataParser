namespace HeroesDataParser.Comparers;

public class UnderscoreFirstComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y))
            return 0;
        if (x is null)
            return -1;
        if (y is null)
            return 1;

        bool xStartsWithUnderscore = x.StartsWith('_');
        bool yStartsWithUnderscore = y.StartsWith('_');

        if (xStartsWithUnderscore && !yStartsWithUnderscore)
            return -1;
        if (!xStartsWithUnderscore && yStartsWithUnderscore)
            return 1;

        return string.Compare(x, y, StringComparison.Ordinal);
    }
}
