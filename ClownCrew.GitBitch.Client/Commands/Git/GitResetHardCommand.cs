using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitResetHardCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;
        private readonly IQuestionAgent _questionAgent;

        public GitResetHardCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness, IQuestionAgent questionAgent)
            : base(settingAgent, "Reset", new[] { "reset hard", "hard reset" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
            _questionAgent = questionAgent;
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

            if (await _questionAgent.AskYesNoAsync("Are you sure you want to reset all changes?"))
            {
                var response = _gitBusiness.Shell("reset --hard", gitRepoPath).ToArray();
                foreach (var line in response)
                {
                    await _talkAgent.SayAsync(line);
                }
            }
            else
            {
                await _talkAgent.SayAsync("All your files have been left untouched.");
            }
        }
    }
}