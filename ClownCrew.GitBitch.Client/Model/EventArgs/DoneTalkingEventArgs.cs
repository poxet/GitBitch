using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class DoneTalkingEventArgs : System.EventArgs
    {
        private readonly Guid _id;

        public DoneTalkingEventArgs(Guid id)
        {
            _id = id;
        }

        public Guid Id { get { return _id; } }
    }
}