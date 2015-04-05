using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public abstract class GitBitchCommand : IGitBitchCommand
    {
        private readonly List<string> _phrases = new List<string>();

        protected GitBitchCommand(string name, string[] phrases = null)
        {
            AddPhrases(phrases ?? new string[] { });
        }

        protected void AddPhrases(string[] phrases)
        {
            foreach (var phrase in phrases)
                _phrases.Add(phrase);

            InvokeRegisterPhraseEvent(phrases);
        }

        public event EventHandler<RegisterPhraseEventArgs> RegisterPhraseEvent;
        public IEnumerable<string> Phrases { get { return _phrases; } }

        public abstract Task ExecuteAsync();

        protected virtual void InvokeRegisterPhraseEvent(string[] phrases)
        {
            var handler = RegisterPhraseEvent;
            if (handler != null) handler(this, new RegisterPhraseEventArgs(phrases));
        }
    }
}