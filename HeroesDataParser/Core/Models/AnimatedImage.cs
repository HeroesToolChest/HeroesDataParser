namespace HeroesDataParser.Core.Models;

public class AnimatedImage
{
    public required string OutputFileName { get; init; }

    public required SixLabors.ImageSharp.Size Size { get; init; }

    public required SixLabors.ImageSharp.Size MaxSize { get; init; }

    public required int Frames { get; init; }

    public required int FrameDelay { get; init; }
}
