using System;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class CloseCommand : GitBitchCommand
    {
        private readonly IQuestionAgent _questionAgent;
        private readonly ITalkAgent _talkAgent;
        public static event EventHandler<EventArgs> CloseDownEvent;

        internal static void InvokeCloseDownEvent()
        {
            var handler = CloseDownEvent;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        public CloseCommand(ISettingAgent settingAgent, IQuestionAgent questionAgent, ITalkAgent talkAgent)
            : base(settingAgent, "Close", new[] { "close", "exit", "quit", "bye", "goodbye", "stop" })
        {
            _questionAgent = questionAgent;
            _talkAgent = talkAgent;
        }

        public async override Task ExecuteAsync(string key, string phrase)
        {
            var response = await _questionAgent.AskYesNoAsync("Are you sure?");
            if (response) 
                InvokeCloseDownEvent();
            else
                await _talkAgent.SayAsync("I am still here for you.");
        }
    }
}