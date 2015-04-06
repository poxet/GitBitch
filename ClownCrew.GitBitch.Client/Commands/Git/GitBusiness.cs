using System.Collections.Generic;
using System.Diagnostics;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitBusiness : IGitBusiness
    {
        public IEnumerable<string> Shell(string command, string gitRepoPath)
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
}