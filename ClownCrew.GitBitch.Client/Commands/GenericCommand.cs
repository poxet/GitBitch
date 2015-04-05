using System;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands
{
    public class GenericCommand : GitBitchCommand
    {
        private readonly Action _action;

        public GenericCommand(ISettingAgent settingAgent, string name, string[] phrases, Action action)
            : base(settingAgent, name, phrases)
        {
            _action = action;
        }

        public override Task ExecuteAsync()
        {
            return Task.Factory.StartNew(_action);
        }
    }
}