using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Commands.Git;
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
                //new GitBitchScanCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.GitRepoAgent),
                new GitOpenCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.QuestionAgent, CompositeRoot.Instance.TalkAgent),
                new GitSelectCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent),
                new GitStatusCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent),
                new GitListCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.TalkAgent)
                //Commit
                //Stage
                //Reset
                //Stash
                //Pop
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}