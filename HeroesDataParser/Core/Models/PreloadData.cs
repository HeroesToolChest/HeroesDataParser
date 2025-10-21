using CASCLib;
using System.Diagnostics.CodeAnalysis;

namespace HeroesDataParser.Core.Models;

public class PreloadData
{
    public CASCConfig? CascConfig { get; set; }

    public ModsInfoFile? ModsInfoFile { get; set; }

    [MemberNotNullWhen(true, nameof(CascConfig))]
    public bool HasCascConfig => CascConfig is not null;

    [MemberNotNullWhen(true, nameof(ModsInfoFile))]
    public bool HasModsInfoFile => ModsInfoFile is not null;
}
