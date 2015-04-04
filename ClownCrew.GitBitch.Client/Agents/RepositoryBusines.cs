using System;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class RepositoryBusines : IRepositoryBusines
    {
        private readonly IDataRepository _dataRepository;
        public event EventHandler<RepositoryAddedEventArgs> RepositoryAddedEvent;

        public RepositoryBusines(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        protected virtual void InvokeRepositoryAddedEvent(IGitRepository gitRepository)
        {
            var handler = RepositoryAddedEvent;
            if (handler != null) handler(this, new RepositoryAddedEventArgs(gitRepository));
        }

        public void Add(string name, string path)
        {
            var gitRepo = new GitRepository(name, path);
            _dataRepository.AddRepository(gitRepo);
            InvokeRepositoryAddedEvent(gitRepo);
        }
    }
}