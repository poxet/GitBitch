using System;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandStateChangedEventArgs : EventArgs
    {
        private readonly string _status;

        public CommandStateChangedEventArgs(string status)
        {
            _status = status;
        }

        public string Status { get { return _status; } }
    }
}