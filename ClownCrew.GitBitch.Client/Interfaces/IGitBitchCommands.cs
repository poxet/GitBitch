using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IGitBitchCommands
    {
        IEnumerable<IGitBitchCommand> Items { get; }
        string Name { get; }
    }
}