using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class StartTalkingEventArgs : System.EventArgs
    {
        private readonly Guid _id;
        private readonly string _name;
        private readonly string _phrase;

        public StartTalkingEventArgs(Guid id, string name, string phrase)
        {
            _id = id;
            _name = name;
            _phrase = phrase;
        }

        public Guid Id { get { return _id; } }
        public string Name { get { return _name; } }
        public string Phrase { get { return _phrase; } }
    }
}