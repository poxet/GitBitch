using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitStashCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;
        private readonly IQuestionAgent _questionAgent;

        public GitStashCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness, IQuestionAgent questionAgent)
            : base(settingAgent, "Stash", new[] { "stash", "stash save", "save stash", "save to stash" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
            _questionAgent = questionAgent;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            var gitRepoPath = await GitCommandTools.GetSelectedPathAsync(_repositoryBusiness, _talkAgent, _settingAgent);
            if (gitRepoPath == null) return;

            var response = _gitBusiness.Shell("stash", gitRepoPath).ToArray();
            foreach (var line in response)
            {
                await _talkAgent.SayAsync(line);
            }
        }
    }
}