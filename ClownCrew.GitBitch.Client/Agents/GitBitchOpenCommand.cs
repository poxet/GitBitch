using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class GitBitchOpenCommand : GitBitchCommand
    {
        private readonly Tuple<string, bool> _greeting;
        private readonly Tuple<string, bool> _bitchName;

        public GitBitchOpenCommand(IRepositoryBusines repositoryBusiness, ISettingAgent settingAgent)
            : base("Open")
        {
            _greeting = settingAgent.GetSetting("Greeting", new Tuple<string, bool>("please", false));
            _bitchName = settingAgent.GetSetting("BitchName", new Tuple<string, bool>(Constants.DefaultBitchName, true));

            repositoryBusiness.RepositoryAddedEvent += RepositoryBusiness_RepositoryAddedEvent;
        }

        private void RepositoryBusiness_RepositoryAddedEvent(object sender, RepositoryAddedEventArgs e)
        {
            var rawPhrases = new List<string> { "Open {RepositoryName}", "Select {RepositoryName}" };
            var actualPhrases = new List<string>();
            foreach (var phrase in rawPhrases)
            {
                var actualPhrase = phrase.Replace("{RepositoryName}", e.GitRepository.Name);

                if (!string.IsNullOrEmpty(_greeting.Item1) && !string.IsNullOrEmpty(_bitchName.Item1))
                {
                    actualPhrases.Add(_greeting.Item1 + " " + actualPhrase + " " + _bitchName.Item1);
                    actualPhrases.Add(_bitchName.Item1 + " " + actualPhrase + " " + _greeting.Item1);
                }

                if (!string.IsNullOrEmpty(_bitchName.Item1) && !_greeting.Item2)
                {
                    actualPhrases.Add(actualPhrase + " " + _bitchName.Item1);
                    actualPhrases.Add(_bitchName.Item1 + " " + actualPhrase);
                }

                if (!string.IsNullOrEmpty(_greeting.Item1) && !_bitchName.Item2)
                {
                    actualPhrases.Add(_greeting.Item1 + " " + actualPhrase);
                    actualPhrases.Add(actualPhrase + " " + _greeting.Item1);
                }

                if (!_greeting.Item2 && !_bitchName.Item2)
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