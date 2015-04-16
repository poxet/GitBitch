using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Windows
{
    public class WindowsCommands : IGitBitchCommands
    {
        private readonly List<IGitBitchCommand> _commands;

        public WindowsCommands()
        {
            _commands = new List<IGitBitchCommand>
            {
                //new GenericCommand(CompositeRoot.Instance.SettingAgent, "Copy", new [] { "copy" }, () => { Clipboard.SetText("Hello, clipboard"); }),
                //new GitBitchCommand("Paste", new List<string> { "paste" }),
                CompositeRoot.Instance.Resolve<LockMachineCommand>(),
                //new LockMachineCommand(CompositeRoot.Instance.SettingAgent)
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
        public string Name { get { return "Windows"; } }
    }
}