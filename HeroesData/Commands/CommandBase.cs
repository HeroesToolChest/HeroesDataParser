using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace HeroesData.Commands
{
    internal abstract class CommandBase
    {
        protected CommandBase(CommandLineApplication app)
        {
            CommandLineApplication = app;
        }

        protected string AppPath { get; } = Path.GetDirectoryName(AppContext.BaseDirectory) ?? string.Empty;
        protected CommandLineApplication CommandLineApplication { get; }
    }
}
