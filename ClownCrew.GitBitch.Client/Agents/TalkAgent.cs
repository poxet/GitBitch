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
        public event EventHandler<SayEventArgs> SayEvent;

        public TalkAgent(ISettingAgent settingAgent)
        {
            _settingAgent = settingAgent;
        }

        private void InvokeSayEvent(string name, string phrase)
        {
            var handler = SayEvent;
            if (handler != null) 
                handler(null, new SayEventArgs(name, phrase));
        }

        //public string StartSay(string phrase)
        //{
        //    var actualPhrase = phrase;
        //    DoSay(actualPhrase);
        //    return actualPhrase;
        //}

        public async Task<string> SayAsync(string phrase)
        {
            var actualPhrase = phrase;
            await DoSay(actualPhrase);
            return actualPhrase;
        }

        private async Task DoSay(string actualPhrase)
        {
            var bitchName = _settingAgent.GetSetting("BitchName", Constants.DefaultBitchName);
            InvokeSayEvent(bitchName, actualPhrase);

            //var task = new Task();

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
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            var qa = new QuestionAgent(this, _settingAgent);
            return await qa.AskAsync(question, alternatives, millisecondsTimeout);
        }
    }
}