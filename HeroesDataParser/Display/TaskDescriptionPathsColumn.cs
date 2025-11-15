using Spectre.Console.Rendering;

namespace HeroesDataParser.Display;

public class TaskDescriptionPathsColumn : ProgressColumn
{
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        return AnsiConsoleHelpers.GetFilePath(task.Description)
            .RightJustified();
    }
}
