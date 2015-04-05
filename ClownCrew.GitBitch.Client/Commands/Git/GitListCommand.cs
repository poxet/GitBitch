using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Commands.Application;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitListCommand : GitBitchCommand
    {
        private readonly ITalkAgent _talkAgent;

        public GitListCommand(ISettingAgent settingAgent, ITalkAgent talkAgent)
            : base(settingAgent, "List", new[] { "list repos", "list repositories", "what repositories are there" })
        {
            _talkAgent = talkAgent;
        }

        public async override Task ExecuteAsync(string phrase)
        {
            var repos = _settingAgent.GetSettings<string>("Repositories");
            await _talkAgent.SayAsync(string.Format("You can choose between " + repos.Keys.ToAndList() + "."));
        }
    }
}