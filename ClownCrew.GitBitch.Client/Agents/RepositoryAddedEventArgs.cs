using System;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class RepositoryAddedEventArgs : EventArgs
    {
        private readonly IGitRepository _gitRepository;

        public RepositoryAddedEventArgs(IGitRepository gitRepository)
        {
            _gitRepository = gitRepository;
        }

        public IGitRepository GitRepository { get { return _gitRepository; } }
    }
}