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
            : base(settingAgent, "Commit", new[] { "commit", "stage and commit", "stage commit", "amend" })
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

            var statusResponse = _gitBusiness.Shell("status", gitRepoPath).ToArray();
            if (!statusResponse.Any(x => x.Contains("are to be committed") || x.Contains("have changes")))
            {
                await _talkAgent.SayAsync("There are no changes to commit.");
                return;
            }

            var extraFlag = string.Empty;
            string commitMessage = null;

            if (phrase.Contains("stage"))
            {
                extraFlag += " -a";
            }
            
            if (phrase.Contains("amend"))
            {
                extraFlag += " --amend";
                commitMessage = "--no-edit";
            }

            //TODO: Fix threading so that this part start to work.
            //if (string.IsNullOrEmpty(commitMessage))
            //    commitMessage = "-m \"" + await _questionAgent.AskStringAsync("Enter a commit message") + "\"";

            var retry = true;
            while (retry)
            {
                retry = false;

                var response = _gitBusiness.Shell("commit " + commitMessage + extraFlag, gitRepoPath).ToArray();
                if (response.Any(x => x.Contains("no changes added to commit")))
                {
                    var stage = await _questionAgent.AskYesNoAsync("No files has been staged yet. Do you want to stage all changes and commit all that?");
                    if (stage)
                    {
                        var stageResponse = _gitBusiness.Shell("add .", gitRepoPath).ToArray();
                        retry = true;
                    }
                    else
                    {
                        await _talkAgent.SayAsync("Allright.");
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