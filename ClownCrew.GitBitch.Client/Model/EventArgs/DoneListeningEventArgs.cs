using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class DoneListeningEventArgs : System.EventArgs
    {
        private readonly Guid _listenId;

        public DoneListeningEventArgs(Guid listenId)
        {
            _listenId = listenId;
        }

        public Guid ListenId { get { return _listenId; } }
    }
}