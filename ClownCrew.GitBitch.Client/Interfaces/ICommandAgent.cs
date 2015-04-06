using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ICommandAgent
    {
        void ClearCommands();
        void RegisterCommands(IGitBitchCommands gitBitchCommands);
        IEnumerable<IGitBitchCommand> Commands { get; }
    }
}