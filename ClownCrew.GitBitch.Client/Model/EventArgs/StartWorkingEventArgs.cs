using System;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class StartWorkingEventArgs : System.EventArgs
    {
        private readonly Guid _workId;
        private readonly string _name;

        public StartWorkingEventArgs(Guid workId, string name)
        {
            _workId = workId;
            _name = name;
        }

        public Guid WorkId { get { return _workId; } }
        public string Name { get { return _name; } }
    }
}