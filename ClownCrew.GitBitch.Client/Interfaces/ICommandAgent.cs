using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ICommandAgent
    {
        Task RegisterAsync(IGitBitchCommands gitBitchCommands);
        IEnumerable<IGitBitchCommand> Commands { get; }
    }
}