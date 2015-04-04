using System;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class RegisterPhraseEventArgs : EventArgs
    {
        private readonly string[] _phrases;

        public RegisterPhraseEventArgs(string[] phrases)
        {
            _phrases = phrases;
        }

        public string[] Phrases { get { return _phrases; } }
    }
}