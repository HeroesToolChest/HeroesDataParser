namespace HeroesDataParser.Core;

public interface IGameStringTextService
{
    Dictionary<string, string> ValByStyleConstantName { get; }

    Dictionary<string, string> ValByStyleName { get; }

    bool ShouldExtractFontValues { get; }

    /// <summary>
    /// Create a new tooltip description.
    /// </summary>
    /// <param name="text">The text to create the tooltip description.</param>
    /// <returns>A <see cref="GameStringText"/>.</returns>
    GameStringText GetGameStringText(string text);

    /// <summary>
    /// Gets a tooltip description from an id. Looks up the id in the gamestrings and parses it.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>A <see cref="GameStringText"/>.</returns>
    GameStringText? GetGameStringTextFromId(string id);

    /// <summary>
    /// Gets a tooltip description from a <see cref="StormGameString"/>.
    /// </summary>
    /// <param name="gamestring">A <see cref="StormGameString"/>, which is an unparsed gamestring.</param>
    /// <returns>A <see cref="GameStringText"/>.</returns>
    GameStringText? GetGameStringTextFromStormGameString(StormGameString gamestring);

    /// <summary>
    /// Gets a tooltip description from an unparsed gamestring.
    /// </summary>
    /// <param name="gamestring">An unparsed gamestring.</param>
    /// <returns>A <see cref="GameStringText"/>.</returns>
    GameStringText? GetGameStringTextFromGameString(string gamestring);

    /// <summary>
    /// Gets an unparsed gamestring from an id.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns>An unparsed gamestring or <see langword="null"/> if not found.</returns>
    string? GetStormGameString(string id);
}
