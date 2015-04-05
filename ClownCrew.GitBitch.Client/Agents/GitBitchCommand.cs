using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public abstract class GitBitchCommand : IGitBitchCommand
    {
        private readonly string _name;
        private readonly Tuple<string, bool> _greeting;
        private readonly Tuple<string, bool> _bitchName;

        private readonly List<string> _phrases = new List<string>();

        protected GitBitchCommand(ISettingAgent settingAgent, string name, string[] phrases = null)
        {
            _name = name;
            _greeting = settingAgent.GetSetting("Greeting", new Tuple<string, bool>("please", false));

            var bitchName = settingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);
            var requireBitchName = settingAgent.GetSetting(Constants.RequireBitchName, true);
            _bitchName = new Tuple<string, bool>(bitchName, requireBitchName);

            AddPhrases(phrases ?? new string[] { });
        }

        protected void AddPhrases(IEnumerable<string> rawPhrases)
        {
            var actualPhrases = new List<string>();
            foreach (var phrase in rawPhrases)
            {
                if (!string.IsNullOrEmpty(_greeting.Item1) && !string.IsNullOrEmpty(_bitchName.Item1))
                {
                    actualPhrases.Add(_greeting.Item1 + " " + phrase + " " + _bitchName.Item1);
                    actualPhrases.Add(_bitchName.Item1 + " " + phrase + " " + _greeting.Item1);
                }

                if (!string.IsNullOrEmpty(_bitchName.Item1) && !_greeting.Item2)
                {
                    actualPhrases.Add(phrase + " " + _bitchName.Item1);
                    actualPhrases.Add(_bitchName.Item1 + " " + phrase);
                }

                if (!string.IsNullOrEmpty(_greeting.Item1) && !_bitchName.Item2)
                {
                    actualPhrases.Add(_greeting.Item1 + " " + phrase);
                    actualPhrases.Add(phrase + " " + _greeting.Item1);
                }

                if (!_greeting.Item2 && !_bitchName.Item2)
                    actualPhrases.Add(phrase);
            }

            foreach (var phrase in actualPhrases)
                _phrases.Add(phrase);

            InvokeRegisterPhraseEvent(actualPhrases.ToArray());
        }

        public event EventHandler<RegisterPhraseEventArgs> RegisterPhraseEvent;
        public string Name { get { return _name; } }
        public IEnumerable<string> Phrases { get { return _phrases; } }

        public abstract Task ExecuteAsync();

        protected virtual void InvokeRegisterPhraseEvent(string[] phrases)
        {
            var handler = RegisterPhraseEvent;
            if (handler != null) handler(this, new RegisterPhraseEventArgs(phrases));
        }
    }
}