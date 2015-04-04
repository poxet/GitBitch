using System.Threading.Tasks;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ICommandAgent
    {
        Task RegisterAsync(IGitBitchCommands gitBitchCommands);
    }
}