using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser;

internal class ImageRelativePath : RelativeFilePath
{
    [SetsRequiredMembers]
    public ImageRelativePath(IElementObject elementObject, RelativeFilePath relativeFilePath)
    {
        Id = elementObject.Id;
        FilePath = relativeFilePath.FilePath;
        MpqFilePath = relativeFilePath.MpqFilePath;
    }

    /// <summary>
    /// Gets the id of the element.
    /// </summary>
    public string Id { get; }
}
