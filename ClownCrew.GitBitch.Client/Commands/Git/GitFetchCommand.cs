using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitCommitCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;
        private readonly IQuestionAgent _questionAgent;

        public GitCommitCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness, IQuestionAgent questionAgent)
            : base(settingAgent, "Commit", new[] { "commit" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
            _questionAgent = questionAgent;
        }

        public override async Task ExecuteAsync(string key)
        {
            //TODO: Duplicate code
            var gitRepoPath = _repositoryBusiness.GetSelectedPath();
            if (string.IsNullOrEmpty(gitRepoPath))
            {
                await _talkAgent.SayAsync("You need to select a repository before you can ask for status.");
                return;
            }

            var response = _gitBusiness.Shell("commit", gitRepoPath).ToArray();
            if (response[response.Length - 2].Contains("no changes added to commit"))
            {
                await _talkAgent.SayAsync("No files has been staged so there is nothing to commit.");
            }
            else
            {
                foreach (var line in response)
                {
                    await _talkAgent.SayAsync(line);
                }
            }
        }
    }

    public class GitFetchCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;
        private readonly IQuestionAgent _questionAgent;

        public GitFetchCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness, IQuestionAgent questionAgent)
            : base(settingAgent, "Fetch", new[] { "fetch" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
            _questionAgent = questionAgent;
        }

        public override async Task ExecuteAsync(string key)
        {
            //TODO: Duplicate code
            var gitRepoPath = _repositoryBusiness.GetSelectedPath();
            if (string.IsNullOrEmpty(gitRepoPath))
            {
                await _talkAgent.SayAsync("You need to select a repository before you can ask for status.");
                return;
            }

            var response = _gitBusiness.Shell("fetch", gitRepoPath).ToArray();
            if (!response.Any())
            {
                await _talkAgent.SayAsync("There is nothing to fetch from origin.");
            }
            else
            {
                foreach (var line in response)
                {
                    await _talkAgent.SayAsync(line);
                }
            }
        }
    }
}