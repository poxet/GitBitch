using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitBusiness : IGitBusiness
    {
        private readonly IEventHub _eventHub;

        public GitBusiness(IEventHub eventHub)
        {
            _eventHub = eventHub;
        }

        public IEnumerable<string> Shell(string command, string gitRepoPath)
        {
            var talkId = Guid.NewGuid();
            try
            {
                _eventHub.InvokeStartWorkingEvent(talkId, command);

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

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        //TODO: Why was this one needed in the first place? (Had something to do with push or fetch
                        //if (count && counter != 0)
                        //{
                        //    yield return string.Format(msg, counter);
                        //    count = false;
                        //}

                        continue;
                    }

                    if (line == "On branch master")
                    {
                        continue;
                    }

                    if (line == "Changes not staged for commit:")
                    {
                        if (count) yield return string.Format(msg, counter);
                        msg = "{0} files have changes.";
                        count = true;
                        counter = 0;
                    }
                    else if (line == "Changes to be committed:")
                    {
                        if (count) yield return string.Format(msg, counter);
                        msg = "{0} files are to be committed.";
                        count = true;
                        counter = 0;
                    }
                    else if (line == "Unstaged changes after reset:")
                    {
                        if (count) yield return string.Format(msg, counter);
                        msg = "{0} files have been unstaged.";
                        count = true;
                        counter = 0;
                    }
                    else
                    {
                        if (count)
                        {
                            if (line.StartsWith("\t") || line.StartsWith("M")) counter++;
                        }
                        else if (!line.StartsWith("  ("))
                        {
                            yield return line;
                        }
                    }
                }

                if (count) yield return string.Format(msg, counter);

                process.WaitForExit();

                if (process.ExitCode != 0) yield return "Exited with error code " + process.ExitCode + ".";

                while (!process.StandardError.EndOfStream)
                {
                    var line = process.StandardError.ReadLine();
                    //if (line == "fatal: Not a git repository (or any of the parent directories): .git")
                    //    yield return "No git repository selected.";
                    //else
                    yield return line;
                }
            }
            finally
            {
                _eventHub.InvokeDoneWorkingEvent(talkId);
            }
        }
    }
}