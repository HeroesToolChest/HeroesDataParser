using Microsoft.Extensions.CommandLineUtils;
using System.IO;
using System.Reflection;

namespace HeroesData.Commands
{
    internal abstract class CommandBase
    {
        protected CommandBase(CommandLineApplication app)
        {
            CommandLineApplication = app;
        }

        protected string AppPath { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        protected CommandLineApplication CommandLineApplication { get; }
    }
}
