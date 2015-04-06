using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class StartListeningEventArgs : System.EventArgs
    {
        private readonly Guid _listenId;

        public StartListeningEventArgs(Guid listenId)
        {
            _listenId = listenId;
        }

        public Guid ListenId { get { return _listenId; } }
    }
}