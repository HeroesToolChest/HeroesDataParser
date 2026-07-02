using Spectre.Console.Rendering;

namespace HeroesDataParser.Display;

public sealed class ItemsProgressColumn : ProgressColumn
{
    /// <summary>
    /// Gets or sets the style for a non-complete task.
    /// </summary>
    public Style Style { get; set; } = Style.Plain;

    /// <summary>
    /// Gets or sets the style for a completed task.
    /// </summary>
    public Style CompletedStyle { get; set; } = Color.Green;

    /// <summary>
    /// Gets or sets the style for a incompleted task.
    /// </summary>
    public Style IncompletedStyle { get; set; } = Color.Yellow;

    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        Style style;
        int percentage = (int)task.Percentage;

        if (task.IsFinished && percentage == 100)
            style = CompletedStyle;
        else if (task.IsFinished && percentage < 100)
            style = IncompletedStyle;
        else
            style = Style.Plain;

        return new Text($"{task.Value} / {task.MaxValue}", style);
    }
}
