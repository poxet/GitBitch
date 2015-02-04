using System;
using System.Speech.Synthesis;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    class TalkAgent : ITalkAgent
    {
        public event EventHandler<EventArgs> SayEvent;

        private void InvokeSayEvent(string phrase)
        {
            var handler = SayEvent;
            if (handler != null) 
                handler(null, EventArgs.Empty);
        }

        public string Say(string phrase)
        {
            var builder = new PromptBuilder();
            builder.StartSentence();
            builder.AppendText(phrase);
            builder.EndSentence();

            using (var synthesizer = new SpeechSynthesizer())
            {
                //synthesizer.SelectVoice("Microsoft David Desktop");
                //synthesizer.SelectVoice("Microsoft Hazel Desktop");
                synthesizer.SelectVoice("Microsoft Zira Desktop");
                synthesizer.Speak(builder);
            }

            return phrase;
        }
    }
}
