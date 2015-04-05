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
                new HelpCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.CommandAgent),
                new CloseCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.QuestionAgent, CompositeRoot.Instance.TalkAgent),
                new ChangeNameCommand(CompositeRoot.Instance.SettingAgent),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}