using System;
using System.ComponentModel;

namespace Heroes.Icons.Parser.Models
{
    [Flags]
    public enum AbilityTier
    {
        [Description("Basic Ability")]
        Basic,
        [Description("Heroic Ability")]
        Heroic,
        [Description("Trait Ability")]
        Trait,
        [Description("Mount Ability")]
        Mount,
        [Description("Activable Ability")]
        Activable,
    }
}
