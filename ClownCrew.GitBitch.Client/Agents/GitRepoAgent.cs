using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class GitRepoAgent : IGitRepoAgent
    {
        private readonly ITalkAgent _talkAgent;
        private readonly IRepositoryBusines _repositoryBusines;

        public GitRepoAgent(ITalkAgent talkAgent, IRepositoryBusines repositoryBusines)
        {
            _talkAgent = talkAgent;
            _repositoryBusines = repositoryBusines;
        }

        public async Task SearchAsync()
        {
            var drives = System.IO.Directory.GetLogicalDrives();
            foreach (var drive in drives)
            {
                var gitFolders = FindGitFolders(drive);
                foreach (var gitFolder in gitFolders)
                {
                    var gitFolderPath = gitFolder.Replace("\\.git", string.Empty);
                    var gitRepoName = gitFolderPath.Substring(gitFolderPath.LastIndexOf("\\", StringComparison.Ordinal) + 1);
                    await _talkAgent.SayAsync("Found git repo " + gitRepoName + ".");
                    _repositoryBusines.Add(gitRepoName, gitFolderPath);
                }
            }
        }

        private IEnumerable<string> FindGitFolders(string drive)
        {
            string[] directories;
            try
            {
                directories = System.IO.Directory.GetDirectories(drive);
            }
            catch (UnauthorizedAccessException)
            {
                yield break;
            }
            catch (Exception exception)
            {
                throw;
            }

            foreach (var directory in directories)
            {
                if (directory.EndsWith("\\.git"))
                    yield return directory;

                var subDirs = FindGitFolders(directory);
                foreach (var subDir in subDirs)
                    yield return subDir;
            }
        }
    }
}