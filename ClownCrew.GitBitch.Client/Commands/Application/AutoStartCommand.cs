using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class AutoStartCommand : GitBitchCommand
    {
        private readonly IQuestionAgent _questionAgent;
        private readonly ITalkAgent _talkAgent;

        public AutoStartCommand(ISettingAgent settingAgent,IQuestionAgent questionAgent, ITalkAgent talkAgent)
            : base(settingAgent, "Autostart", new[] { "enable autostart", "disable autostart", "autostart on", "autostart off", "autostart" })
        {
            _questionAgent = questionAgent;
            _talkAgent = talkAgent;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            bool autoStart;
            if (phrase.Contains("enable") || phrase.Contains("on"))
            {
                autoStart = true;
            }
            else if (phrase.Contains("disable") || phrase.Contains("off"))
            {
                autoStart = false;
            }
            else
            {
                autoStart = await _questionAgent.AskYesNoAsync("Do you want GitBitch to start automatically when windows starts?");
            }

            await _talkAgent.SayAsync("Autostart is " + (autoStart ? "enabled" : "disabled") + ".");

            _settingAgent.UseAutoStart(autoStart);
        }
    }
}