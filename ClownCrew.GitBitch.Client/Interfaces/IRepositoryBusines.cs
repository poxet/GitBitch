using System;
using ClownCrew.GitBitch.Client.Agents;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IRepositoryBusines
    {
        event EventHandler<RepositoryAddedEventArgs> RepositoryAddedEvent;
        void Add(string name, string path);
        void Select(string name);
        string GetSelectedPath();
    }
}