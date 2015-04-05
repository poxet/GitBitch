using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitBitchSelectCommand : GitBitchCommand
    {
        public GitBitchSelectCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness)
            : base(settingAgent, "Select", new string[] { })
        {
            repositoryBusiness.RepositoryAddedEvent += RepositoryBusiness_RepositoryAddedEvent;
        }

        private void RepositoryBusiness_RepositoryAddedEvent(object sender, RepositoryAddedEventArgs e)
        {
            var rawPhrases = new[] { "select {RepositoryName}" };
            var actualPhrases = new List<string>();
            foreach (var phrase in rawPhrases)
            {
                var actualPhrase = phrase.Replace("{RepositoryName}", e.GitRepository.Name);
                actualPhrases.Add(actualPhrase);
            }

            AddPhrases(actualPhrases.ToArray());
        }

        public async override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}