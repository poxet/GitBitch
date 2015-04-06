using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class TalkAgent : ITalkAgent
    {
        private readonly ISettingAgent _settingAgent;
        public event EventHandler<StartSayEventArgs> StartSayEvent;
        public event EventHandler<SayCompleteEventArgs> SayCompleteEvent;

        public TalkAgent(ISettingAgent settingAgent)
        {
            _settingAgent = settingAgent;
        }

        private void InvokeStartSayEvent(Guid id, string name, string phrase)
        {
            var handler = StartSayEvent;
            if (handler != null) 
                handler(null, new StartSayEventArgs(id, name, phrase));
        }

        protected virtual void InvokeSayCompleteEvent(Guid id)
        {
            var handler = SayCompleteEvent;
            if (handler != null) handler(this, new SayCompleteEventArgs(id));
        }

        public async Task<string> SayAsync(string phrase)
        {
            var actualPhrase = phrase;
            await DoSay(actualPhrase);
            return actualPhrase;
        }

        private async Task DoSay(string actualPhrase)
        {
            var bitchName = _settingAgent.GetSetting("BitchName", Constants.DefaultBitchName);
            var newGuid = Guid.NewGuid();
            InvokeStartSayEvent(newGuid, bitchName, actualPhrase);

            var task = new Task(() =>
            {
                var builder = new PromptBuilder();
                builder.StartSentence();
                builder.AppendText(actualPhrase);
                builder.EndSentence();

                using (var synthesizer = new SpeechSynthesizer())
                {
                    //synthesizer.SelectVoice("Microsoft David Desktop");
                    //synthesizer.SelectVoice("Microsoft Hazel Desktop");
                    synthesizer.SelectVoice("Microsoft Zira Desktop");
                    synthesizer.Speak(builder);
                }
            });

            task.Start();
            await task;
            InvokeSayCompleteEvent(newGuid);
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            var qa = new QuestionAgent(this, _settingAgent);
            return await qa.AskAsync(question, alternatives, millisecondsTimeout);
        }
    }
}