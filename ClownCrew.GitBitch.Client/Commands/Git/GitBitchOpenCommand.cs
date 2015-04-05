using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitBitchOpenCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly IQuestionAgent _questionAgent;
        private readonly ITalkAgent _talkAgent;

        public GitBitchOpenCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, IQuestionAgent questionAgent, ITalkAgent talkAgent)
            : base(settingAgent, "Open", new[] { "open repo", "open repository" })
        {
            _repositoryBusiness = repositoryBusiness;
            _questionAgent = questionAgent;
            _talkAgent = talkAgent;
        }

        public async override Task ExecuteAsync()
        {
            string path = null;
            while (path == null)
            {
                path = await _questionAgent.AskFolderAsync("Please select the folder where the repository is located.");

                if (path == null)
                {
                    await _talkAgent.SayAsync("Okey, so you changed your mind.");
                    return;
                }

                if (!System.IO.Directory.Exists(path + "\\.git"))
                {
                    await _talkAgent.SayAsync("I cannot find the '.git' folder in this path.");
                    path = null;
                }
            }

            var name = await _questionAgent.AskStringAsync("What name do you want for the repository?");
            if (!string.IsNullOrEmpty(name))
            {
                _settingAgent.SetSetting("Repositories", name, path);

                //TODO: Store informaton about this repo in registry
                //TODO: Select this repository
                await _talkAgent.SayAsync("I have selected repository " + name + " for you.");
                _repositoryBusiness.Select(name);
            }
            else
            {
                await _talkAgent.SayAsync("Open repository was aborted.");
            }
        }
    }
}