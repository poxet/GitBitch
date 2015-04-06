using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitUnstageCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;

        public GitUnstageCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness)
            : base(settingAgent, "Unstage", new[] { "unstage", "remove", "soft reset", "reset soft" })
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

            var before = _gitBusiness.Shell("status .", gitRepoPath).ToArray();

            var response = _gitBusiness.Shell("reset HEAD .", gitRepoPath).ToArray();

            if (!before.Last().Contains("files have changes"))
            {
                foreach (var line in response)
                    await _talkAgent.SayAsync(line);
            }
            else
            {
                await _talkAgent.SayAsync("There are no staged files.");
            }
        }
    }
}