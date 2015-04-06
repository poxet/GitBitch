using System;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public class SayCompleteEventArgs : EventArgs
    {
        private readonly Guid _id;

        public SayCompleteEventArgs(Guid id)
        {
            _id = id;
        }

        public Guid Id { get { return _id; } }
    }
}