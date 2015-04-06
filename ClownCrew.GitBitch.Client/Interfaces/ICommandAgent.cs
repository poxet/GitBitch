using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ICommandAgent
    {
        event EventHandler<CommandStateChangedEventArgs> CommandStateChangedEvent;
        Task ClrearAsync();
        Task RegisterAsync(IGitBitchCommands gitBitchCommands);
        IEnumerable<IGitBitchCommand> Commands { get; }
    }
}