using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitStatusCommand : GitBitchCommand
    {
        private readonly IRepositoryBusines _repositoryBusiness;
        private readonly ITalkAgent _talkAgent;

        public GitStatusCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent)
            : base(settingAgent, "Status", new[] { "status" })
        {
            _repositoryBusiness = repositoryBusiness;
            _talkAgent = talkAgent;
        }

        public async override Task ExecuteAsync()
        {
            var gitRepoPath = _repositoryBusiness.GetSelectedPath();

            var response = GitShell("status", gitRepoPath);
            foreach (var line in response)
            {
                await _talkAgent.SayAsync(line);
            }
        }

        private static IEnumerable<string> GitShell(string command, string gitRepoPath)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            startInfo.FileName = "git";
            startInfo.Arguments = string.Format("-C {0} " + command, gitRepoPath);

            var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            var counter = 0;
            var count = false;
            var msg = string.Empty;

            //process.StandardInput
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                if (line == "On branch master")
                {
                    continue;
                }

                if (line == "Changes not staged for commit:")
                {
                    if (count)
                        yield return string.Format(msg, counter);
                    msg = "{0} files have changes.";
                    count = true;
                    counter = 0;
                }
                else if (line == "Changes to be committed:")
                {
                    if (count)
                        yield return string.Format(msg, counter);
                    msg = "{0} files are to be committed.";
                    count = true;
                    counter = 0;
                }
                else if (line == "Unstaged changes after reset:")
                {
                    if (count)
                        yield return string.Format(msg, counter);
                    msg = "{0} files have been unstaged.";
                    count = true;
                    counter = 0;
                }
                else
                {
                    if (count)
                    {
                        if (line.StartsWith("\t") || line.StartsWith("M"))
                            counter++;
                    }
                    else if (!line.StartsWith("  ("))
                    {
                        yield return line;
                    }
                }
            }

            if (count)
                yield return string.Format(msg, counter);

            process.WaitForExit();

            if (process.ExitCode != 0)
                yield return "Error number " + process.ExitCode;

            while (!process.StandardError.EndOfStream)
            {
                var line = process.StandardError.ReadLine();
                if (line == "fatal: Not a git repository (or any of the parent directories): .git")
                    yield return "No git repository selected.";
                else
                    yield return line;
            }
        }
    }

    public class GitSelectCommand : GitBitchCommand
    {
        public GitSelectCommand(ISettingAgent settingAgent, IRepositoryBusines repositoryBusiness)
            : base(settingAgent, "Select", new string[] { })
        {
            repositoryBusiness.RepositoryAddedEvent += RepositoryBusiness_RepositoryAddedEvent;
        }

        private void RepositoryBusiness_RepositoryAddedEvent(object sender, RepositoryAddedEventArgs e)
        {
            var rawPhrases = new[] { "select {RepositoryName}" };
            var actualPhrases = new List<string>();
            foreach (var phrase in rawPhrases)
            {
                var actualPhrase = phrase.Replace("{RepositoryName}", e.GitRepository.Name);
                actualPhrases.Add(actualPhrase);
            }

            AddPhrases(actualPhrases.ToArray());
        }

        public async override Task ExecuteAsync()
        {
            throw new NotImplementedException();
        }
    }
}