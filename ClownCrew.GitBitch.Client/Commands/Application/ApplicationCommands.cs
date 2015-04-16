using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class ApplicationCommands : IGitBitchCommands
    {
        private readonly List<IGitBitchCommand> _commands;

        public ApplicationCommands()
        {
            _commands = new List<IGitBitchCommand>
            {
                CompositeRoot.Instance.Resolve<HelpCommand>(),
                CompositeRoot.Instance.Resolve<CloseCommand>(),
                CompositeRoot.Instance.Resolve<ChangeNameCommand>(),
                CompositeRoot.Instance.Resolve<AutoStartCommand>(),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}