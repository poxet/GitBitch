using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class WindowsCommands : IGitBitchCommands
    {
        private readonly List<IGitBitchCommand> _commands;

        public WindowsCommands()
        {
            _commands = new List<IGitBitchCommand>
            {
                //new GitBitchCommand("Copy", new List<string> { "copy" }),
                //new GitBitchCommand("Paste", new List<string> { "paste" }),
                //new GitBitchCommand("Find", new List<string> { "find" }),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}