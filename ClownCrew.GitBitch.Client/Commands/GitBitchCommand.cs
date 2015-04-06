using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Commands
{
    public abstract class GitBitchCommand : IGitBitchCommand
    {
        private readonly string _name;
        private readonly Tuple<string, bool> _greeting;
        private readonly Tuple<string, bool> _bitchName;
        private readonly Dictionary<string, List<string>> _phrases = new Dictionary<string, List<string>>();

        //TODO: Try to not use the setting agent, use a business class instead
        protected readonly ISettingAgent _settingAgent;

        protected GitBitchCommand(ISettingAgent settingAgent, string name, string[] phrases = null)
        {
            _settingAgent = settingAgent;
            _name = name;
            var greetingName = settingAgent.GetSetting(Constants.Greeting, Constants.DefaultGreeting);
            var requireGreeting = settingAgent.GetSetting(Constants.RequireGreeting, false);
            _greeting = new Tuple<string, bool>(greetingName, requireGreeting);

            var bitchName = settingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);
            var requireBitchName = settingAgent.GetSetting(Constants.RequireBitchName, true);
            _bitchName = new Tuple<string, bool>(bitchName, requireBitchName);

            AddPhrases(string.Empty, phrases ?? new string[] { });
        }

        protected void AddPhrases(string key, IEnumerable<string> rawPhrases)
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

                if (!_greeting.Item2 && !_bitchName.Item2) actualPhrases.Add(phrase);
            }

            _phrases.Add(key, actualPhrases);

            InvokeRegisterPhraseEvent(actualPhrases.ToArray());
        }

        public event EventHandler<RegisterPhraseEventArgs> RegisterPhraseEvent;
        public string Name { get { return _name; } }
        public IEnumerable<string> Phrases { get { return _phrases.Values.SelectMany(x => x); } }

        public abstract Task ExecuteAsync(string key);

        public string GetKey(string phrase)
        {
            foreach (var k in _phrases)
            {
                if (k.Value.Any(y => y == phrase))
                {
                    return k.Key;
                }
            }

            throw new InvalidOperationException("Cannot find a match for the phrase.");
        }

        private void InvokeRegisterPhraseEvent(string[] phrases)
        {
            var handler = RegisterPhraseEvent;
            if (handler != null) handler(this, new RegisterPhraseEventArgs(phrases));
        }
    }
}