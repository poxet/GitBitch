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
                CompositeRoot.Instance.Resolve<GitOpenCommand>(),
                CompositeRoot.Instance.Resolve<GitSelectCommand>(),
                CompositeRoot.Instance.Resolve<GitStatusCommand>(),
                CompositeRoot.Instance.Resolve<GitListCommand>(),
                CompositeRoot.Instance.Resolve<GitStageCommand>(),
                CompositeRoot.Instance.Resolve<GitUnstageCommand>(),
                CompositeRoot.Instance.Resolve<GitResetHardCommand>(),
                CompositeRoot.Instance.Resolve<GitPushCommand>(),
                CompositeRoot.Instance.Resolve<GitFetchCommand>(),
                CompositeRoot.Instance.Resolve<GitCommitCommand>(),
                CompositeRoot.Instance.Resolve<GitPullCommand>(),
                CompositeRoot.Instance.Resolve<GitStashCommand>(),
                CompositeRoot.Instance.Resolve<GitStashPopCommand>(),
                CompositeRoot.Instance.Resolve<GitRebaseCommand>(),
            };
        }

        public IEnumerable<IGitBitchCommand> Items { get { return _commands; } }
        public string Name { get { return "Git"; } }
    }
}