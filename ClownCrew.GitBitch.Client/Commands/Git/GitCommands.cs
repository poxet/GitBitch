using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitCommands : IGitBitchCommands
    {
        private readonly List<IGitBitchCommand> _commands;

        public GitCommands()
        {
            _commands = new List<IGitBitchCommand>
            {
                //new GitBitchScanCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.GitRepoAgent),
                new GitOpenCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.QuestionAgent, CompositeRoot.Instance.TalkAgent),
                new GitSelectCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent),
                new GitStatusCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.GitBusiness),
                new GitListCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.TalkAgent),
                new GitStageCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.GitBusiness),
                new GitUnstageCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.GitBusiness),
                new GitResetHardCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.GitBusiness, CompositeRoot.Instance.QuestionAgent),
                new GitPushCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.RepositoryBusines, CompositeRoot.Instance.TalkAgent, CompositeRoot.Instance.GitBusiness, CompositeRoot.Instance.QuestionAgent),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
    }
}