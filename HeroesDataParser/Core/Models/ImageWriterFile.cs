namespace HeroesDataParser.Core.Models;

// for output image paths
[DebuggerDisplay("SubDirectoryPath = {SubDirectoryPath}, FileName = {FileName}")]
public class ImageWriterFile : IEquatable<ImageWriterFile>
{
    /// <summary>
    /// Gets the output subdirectory path where the image is to be created.
    /// </summary>
    public required string SubDirectoryPath { get; init; }

    /// <summary>
    /// Gets the file name (with extension) of the image to be created. This is used as a unique identifier for the image, which may or may not be the actually written file name due to processing.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the id of the element of which this image belongs to.
    /// </summary>
    public required string ElementId { get; init; }

    public required Func<string, Task> ProcessImageFile { get; init; }

    public bool Equals(ImageWriterFile? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(SubDirectoryPath, other.SubDirectoryPath, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is ImageWriterFile other && Equals(other);

    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(FileName, StringComparer.OrdinalIgnoreCase);
        hash.Add(SubDirectoryPath, StringComparer.OrdinalIgnoreCase);

        return hash.ToHashCode();
    }
}
