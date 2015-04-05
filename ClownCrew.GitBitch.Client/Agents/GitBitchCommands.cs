﻿using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class GitBitchCommands : IGitBitchCommands
    {
        private readonly List<IGitBitchCommand> _commands;

        public GitBitchCommands()
        {
            _commands = new List<IGitBitchCommand>
            {
                new GitBitchOpenCommand(CompositeRoot.Instance.RepositoryBusines,CompositeRoot.Instance.SettingAgent),
                //new GitBitchCommand("Status", new List<string> { "status" }),
                //new GitBitchCommand("Commit", new List<string> { "commit" }),
                //new GitBitchCommand("Stage", new List<string> { "stage" }),
                //new GitBitchCommand("Reset", new List<string> { "reset" }),
                //new GitBitchCommand("Stash", new List<string> { "stash" }),
                //new GitBitchCommand("Pop", new List<string> { "pop" }),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}