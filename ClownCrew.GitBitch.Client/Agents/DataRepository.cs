using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class DataRepository : IDataRepository
    {
        private readonly Dictionary<string, IGitRepository> _repositories;

        public DataRepository()
        {
            _repositories = new Dictionary<string, IGitRepository>();
        }

        public void AddRepository(IGitRepository gitRepo)
        {
            _repositories.Add(gitRepo.Name, gitRepo);
        }
    }
}