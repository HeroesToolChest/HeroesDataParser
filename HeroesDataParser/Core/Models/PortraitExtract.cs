namespace HeroesDataParser.Core.Models;

public class PortraitExtract
{
    public required string TextureSheetImage { get; init; }

    /// <summary>
    /// Gets the icon slot 0 reward portrait id.
    /// </summary>
    public required string IconSlotZeroRewardPortraitId { get; init; }

    /// <summary>
    /// Gets the icon slot 1 reward portrait id.
    /// </summary>
    public required string IconSlotOneRewardPortraitId { get; init; }

    /// <summary>
    /// Gets the original file name which is a sha256.
    /// </summary>
    public required string OriginalFileName { get; init; }
}
