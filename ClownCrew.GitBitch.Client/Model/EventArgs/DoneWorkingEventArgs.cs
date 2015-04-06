using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class DoneWorkingEventArgs : System.EventArgs
    {
        private readonly Guid _workId;

        public DoneWorkingEventArgs(Guid workId)
        {
            _workId = workId;
        }

        public Guid WorkId { get { return _workId; } }
    }
}