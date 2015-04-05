using System;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public class SayEventArgs : EventArgs
    {
        public SayEventArgs(string name, string phrase)
        {
            Name = name;
            Phrase = phrase;
        }

        public string Phrase { get; private set; }
        public object Name { get; private set; }
    }
}