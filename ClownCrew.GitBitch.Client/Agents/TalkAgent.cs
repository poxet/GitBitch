using System;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class TalkAgent : ITalkAgent
    {
        public event EventHandler<SayEventArgs> SayEvent;

        private void InvokeSayEvent(string phrase)
        {
            var handler = SayEvent;
            if (handler != null) 
                handler(null, new SayEventArgs(phrase));
        }

        public async Task<string> SayAsync(string phrase)
        {
            var actualPhrase = phrase;

            InvokeSayEvent(actualPhrase);

            var task = Task.Factory.StartNew(() =>
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

            await task;
            return actualPhrase;
        }
    }
}
