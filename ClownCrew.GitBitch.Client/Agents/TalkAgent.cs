using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class TalkAgent : ITalkAgent
    {
        private readonly ISettingAgent _settingAgent;
        private readonly IEventHub _eventHub;

        public TalkAgent(ISettingAgent settingAgent, IEventHub eventHub)
        {
            _settingAgent = settingAgent;
            _eventHub = eventHub;
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
            _eventHub.InvokeStartTalkingEvent(newGuid, bitchName, actualPhrase);

            var task = new Task(() =>
            {
                var builder = new PromptBuilder();
                builder.StartSentence();
                builder.AppendText(actualPhrase);
                builder.EndSentence();

                using (var synthesizer = new SpeechSynthesizer())
                {
                    var voices = synthesizer.GetInstalledVoices();
                    var voice = voices.LastOrDefault(x => x.VoiceInfo.Gender == VoiceGender.Female);
                    if (voice == null) voice = voices.FirstOrDefault();
                    if (voice == null) throw new InvalidOperationException("Cannot find any installed voices.");

                    //synthesizer.SelectVoice("Microsoft David Desktop");
                    //synthesizer.SelectVoice("Microsoft Hazel Desktop");
                    //synthesizer.SelectVoice("Microsoft Zira Desktop");

                    synthesizer.SelectVoice(voice.VoiceInfo.Name);
                    synthesizer.Speak(builder);
                }
            });

            task.Start();
            await task;
            _eventHub.InvokeDoneTalkingEvent(newGuid);
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            var qa = new QuestionAgent(this, _settingAgent, _eventHub);
            return await qa.AskAsync(question, alternatives, millisecondsTimeout);
        }
    }
}