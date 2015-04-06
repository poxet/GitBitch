using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitSelectCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;

        public GitSelectCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent)
            : base(settingAgent, "Select", new string[] { })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;

            var repos = settingAgent.GetSettings<string>("Repositories");
            foreach (var repo in repos)
            {
                AddPhrases(repo.Key, repos.Keys.Select(x => "select " + x));
            }

            repositoryBusiness.RepositoryAddedEvent += RepositoryBusiness_RepositoryAddedEvent;
        }

        private void RepositoryBusiness_RepositoryAddedEvent(object sender, RepositoryAddedEventArgs e)
        {
            var rawPhrases = new[] { "select {RepositoryName}" };
            AddPhrases(e.GitRepository.Name, rawPhrases.Select(phrase => phrase.Replace("{RepositoryName}", e.GitRepository.Name)).ToArray());
        }

        public async override Task ExecuteAsync(string key)
        {
            _repositoryBusiness.Select(key);
            await _talkAgent.SayAsync("Repo " + key + " has been selected.");
        }
    }
}