using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitPushCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;
        private readonly IQuestionAgent _questionAgent;

        public GitPushCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness, IQuestionAgent questionAgent)
            : base(settingAgent, "Push", new[] { "push" })
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

            await _talkAgent.SayAsync("Starting to push.");

            var retry = true;
            while (retry)
            {
                retry = false;

                var response = _gitBusiness.Shell("push", gitRepoPath).ToArray();
                if (response.First().Contains("warning: push.default is unset"))
                {
                    //Set push mode to simple
                    var isSetToSimple = await _questionAgent.AskYesNoAsync("The push behaviour has not been set. Is it allright if I set it to simple?");
                    if (isSetToSimple)
                    {
                        var response2 = _gitBusiness.Shell("config --global push.default simple", gitRepoPath).ToArray();
                        foreach (var line in response2)
                        {
                            await _talkAgent.SayAsync(line);
                        }

                        retry = true;
                    }
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
}