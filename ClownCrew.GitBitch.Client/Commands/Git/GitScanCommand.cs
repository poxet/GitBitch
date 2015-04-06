using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitScanCommand : GitBitchCommand
    {
        private readonly IGitRepoAgent _gitRepoAgent;

        public GitScanCommand(ISettingAgent settingAgent, IGitRepoAgent gitRepoAgent)
            : base(settingAgent, "Scan", new[] { "scan" })
        {
            _gitRepoAgent = gitRepoAgent;
        }

        public async override Task ExecuteAsync(string phrase)
        {
            await CompositeRoot.Instance.TalkAgent.SayAsync("Starting to scan now.");
            await _gitRepoAgent.SearchAsync();
        }
    }
}