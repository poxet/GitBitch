using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IGitBusiness
    {
        IEnumerable<string> Shell(string command, string gitRepoPath);
    }
}