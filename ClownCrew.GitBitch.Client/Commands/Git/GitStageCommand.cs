using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitStageCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;
        private readonly IGitBusiness _gitBusiness;

        public GitStageCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, IGitBusiness gitBusiness)
            : base(settingAgent, "Stage", new[] { "stage", "add" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
            _gitBusiness = gitBusiness;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            var gitRepoPath = await GitCommandTools.GetSelectedPathAsync(_repositoryBusiness, _talkAgent, _settingAgent);
            if (gitRepoPath == null) return;

            var before = _gitBusiness.Shell("status .", gitRepoPath).ToArray();
            var response = _gitBusiness.Shell("add .", gitRepoPath).ToArray();

            if (before.Last().Contains("files have changes"))
            {
                await _talkAgent.SayAsync(string.Format("{0} files have been staged.", before.Last().Split(' ')[0]));
            }
            else
            {
                await _talkAgent.SayAsync("There are no changes to stage.");
            }
        }
    }
}