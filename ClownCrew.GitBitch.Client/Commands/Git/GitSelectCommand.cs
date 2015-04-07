using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var rawPhrases = GetRawList();
                AddPhrases(repo.Key, rawPhrases.Select(x => x.Replace("{RepositoryName}", repo.Key)).ToArray());
            }

            repositoryBusiness.RepositoryAddedEvent += RepositoryBusiness_RepositoryAddedEvent;
        }

        private void RepositoryBusiness_RepositoryAddedEvent(object sender, RepositoryAddedEventArgs e)
        {
            var rawPhrases = GetRawList();
            AddPhrases(e.GitRepository.Name, rawPhrases.Select(x => x.Replace("{RepositoryName}", e.GitRepository.Name)).ToArray());
        }

        private static IEnumerable<string> GetRawList()
        {
            return new[] { "select {RepositoryName}", "select repository {RepositoryName}" };
        }

        public async override Task ExecuteAsync(string key, string phrase)
        {
            _repositoryBusiness.Select(key);
            await _talkAgent.SayAsync("Repo " + key + " has been selected.");
        }
    }
}