using System;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandRegisteredEventArgs : EventArgs
    {
        private readonly string _sectionName;
        private readonly IGitBitchCommand _command;

        public CommandRegisteredEventArgs(string sectionName, IGitBitchCommand command)
        {
            _sectionName = sectionName;
            _command = command;
        }

        public string SectionName { get { return _sectionName; } }
        public IGitBitchCommand Command { get { return _command; } }
    }
}