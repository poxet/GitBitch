using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class GitBitchScanCommand : GitBitchCommand
    {
        private readonly IGitRepoAgent _gitRepoAgent;

        public GitBitchScanCommand(IGitRepoAgent gitRepoAgent)
            : base("Scan", new[] { "scan" })
        {
            _gitRepoAgent = gitRepoAgent;
        }

        public async override Task ExecuteAsync()
        {
            await CompositeRoot.Instance.TalkAgent.SayAsync("Starting to scan now.");
            await _gitRepoAgent.SearchAsync();
        }
    }
}