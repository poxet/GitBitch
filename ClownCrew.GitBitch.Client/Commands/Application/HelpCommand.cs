using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class HelpCommand : GitBitchCommand
    {
        private readonly ITalkAgent _talkAgent;
        private readonly ICommandAgent _commandAgent;

        public HelpCommand(ISettingAgent settingAgent, ITalkAgent talkAgent, ICommandAgent commandAgent)
            : base(settingAgent, "Help", new[] { "help" })
        {
            _talkAgent = talkAgent;
            _commandAgent = commandAgent;
        }

        public override async Task ExecuteAsync(string phrase)
        {
            //TODO: Check the settings to se how you should address the assestent. The help should reclect how you talk to her.
            await _talkAgent.SayAsync("When you talk to me you should always say my name at the beginning or the end of the sentence. You can also start or end the sentence with the world please.");

            var commands = _commandAgent.Commands.Select(x => x.Name).ToAndList();

            await _talkAgent.SayAsync("I understand the following commands. " + commands + ".");
        }
    }
}