using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitRebaseCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;

        public GitRebaseCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness)
            : base(settingAgent, "Rebase", new[] { "rebase" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            var gitRepoPath = await GitCommandTools.GetSelectedPathAsync(_repositoryBusiness, _talkAgent, _settingAgent);
            if (gitRepoPath == null) return;

            var response = _gitBusiness.Shell("rebase", gitRepoPath).ToArray();
            foreach (var line in response)
            {
                await _talkAgent.SayAsync(line);
            }
        }
    }
}