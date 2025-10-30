namespace HeroesDataParser.Core.Models;

// for output image paths
[DebuggerDisplay("SubDirectoryPath = {SubDirectoryPath}, FileName = {FileName}")]
public class ImageWriterPath : IEquatable<ImageWriterPath>
{
    /// <summary>
    /// Gets the output sub-directory path where the image is to be created.
    /// </summary>
    public required string SubDirectoryPath { get; init; }

    /// <summary>
    /// Gets the file name (with extension) of the image to be created.
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Gets the id of the element.
    /// </summary>
    public required string ElementId { get; init; }

    /// <summary>
    /// Gets the relative file path (including the file name) of the image that is to the original image file.
    /// </summary>
    public required string RelativeFilePath { get; init; }

    /// <summary>
    /// Gets the relative file path of the mpq file that contains the <see cref="RelativeFilePath"/>.
    /// </summary>
    public required string? RelativeMpqFilePath { get; init; }

    public bool Equals(ImageWriterPath? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase)
            && string.Equals(SubDirectoryPath, other.SubDirectoryPath, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is ImageWriterPath other && Equals(other);

    public override int GetHashCode()
    {
        HashCode hash = default;
        hash.Add(FileName, StringComparer.OrdinalIgnoreCase);
        hash.Add(SubDirectoryPath, StringComparer.OrdinalIgnoreCase);

        return hash.ToHashCode();
    }
}
