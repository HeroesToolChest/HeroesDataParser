using Spectre.Console.Rendering;

namespace HeroesDataParser.Display;

public class TaskDescriptionPathsColumn : ProgressColumn
{
    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        return new TextPath(task.Description)
            .SeparatorColor(Color.SpringGreen1)
            .StemColor(Color.SteelBlue1_1)
            .LeafColor(Color.Orange1)
            .RightJustified();
    }
}
