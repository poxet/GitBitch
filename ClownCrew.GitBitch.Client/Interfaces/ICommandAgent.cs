using System;
using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Agents;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ICommandAgent
    {
        event EventHandler<CommandRegisteredEventArgs> CommandRegisteredEvent;
        void ClearCommands();
        void RegisterCommands(IGitBitchCommands gitBitchCommands);
        IEnumerable<IGitBitchCommand> Commands { get; }
    }
}