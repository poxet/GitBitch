using System;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Business
{
    public class RepositoryBusines : IRepositoryBusines
    {
        private readonly IDataRepository _dataRepository;
        private readonly ISettingAgent _settingAgent;
        public event EventHandler<RepositoryAddedEventArgs> RepositoryAddedEvent;
        private string _selectedRepositoryName;

        public RepositoryBusines(IDataRepository dataRepository, ISettingAgent settingAgent)
        {
            _dataRepository = dataRepository;
            _settingAgent = settingAgent;
            _selectedRepositoryName = _settingAgent.GetSetting<string>("LastSelected", null);
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

        public void Select(string name)
        {
            _selectedRepositoryName = name;
            _settingAgent.SetSetting("LastSelected", name);
        }

        public string GetSelectedPath()
        {
            var path = _settingAgent.GetSetting<string>("Repositories", _selectedRepositoryName, null);
            return path;
        }
    }
}