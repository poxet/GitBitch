using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitStatusCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;

        public GitStatusCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness)
            : base(settingAgent, "Status", new[] { "status" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            //TODO: Duplicate code
            var gitRepoPath = _repositoryBusiness.GetSelectedPath();
            if (string.IsNullOrEmpty(gitRepoPath))
            {
                await _talkAgent.SayAsync("You need to select a repository before you can ask for status.");
                return;
            }

            var response = _gitBusiness.Shell("status", gitRepoPath);
            foreach (var line in response)
            {
                await _talkAgent.SayAsync(line);
            }
        }
    }
}