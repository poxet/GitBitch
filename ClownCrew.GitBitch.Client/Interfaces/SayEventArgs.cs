using System;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public class SayEventArgs : EventArgs
    {
        public SayEventArgs(string phrase)
        {
            Phrase = phrase;
        }

        public string Phrase { get; private set; }
    }
}