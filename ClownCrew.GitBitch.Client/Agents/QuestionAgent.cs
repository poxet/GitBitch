using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class QuestionAgent : IQuestionAgent
    {
        private readonly AutoResetEvent _responseEvent = new AutoResetEvent(false);
        private readonly ITalkAgent _talkAgent;
        private string _responsePhrase;

        public QuestionAgent(ITalkAgent talkAgent)
        {
            _talkAgent = talkAgent;
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            using (var listenerAgent = new ListenerAgent<T>(alternatives))
            {
                listenerAgent.HeardSomethingEvent += ListenerAgent_HeardSomethingEvent;                

                await _talkAgent.SayAsync(question);

                string response = null;
                var r = await Task.Factory.StartNew(() =>
                {
                    if (!_responseEvent.WaitOne(millisecondsTimeout))
                    {
                        var defaultAlternative = alternatives.FirstOrDefault(x => x.IsDefault) ?? alternatives.First();
                        response = "No answer, so " + defaultAlternative.Phrases.First() + " then.";                        
                        return new Answer<T>(defaultAlternative.Response);
                    }

                    var selectedAlternative = alternatives.First(x => x.Phrases.Any(y => y == _responsePhrase));
                    return new Answer<T>(selectedAlternative.Response);
                });

                if (!string.IsNullOrEmpty(response))
                    await _talkAgent.SayAsync(response);

                return r;
            }
        }

        public async Task<bool> AskYesNoAsync(string question, int millisecondsTimeout = 3000)
        {
            var questionAnswerAlternatives = new List<QuestionAnswerAlternative<bool>>
            {
                new QuestionAnswerAlternative<bool>
                {
                    Phrases = new List<string> { "Yes" },
                    IsDefault = false,
                    Response = true
                },
                new QuestionAnswerAlternative<bool>
                {
                    Phrases = new List<string> { "No" },
                    IsDefault = true,
                    Response = false
                }
            };
            return (await AskAsync(question, questionAnswerAlternatives, millisecondsTimeout)).Response;
        }

        private void ListenerAgent_HeardSomethingEvent(object sender, HeardSomethingEventArgs e)
        {
            _responsePhrase = e.Phrase;
            _responseEvent.Set();
        }        
    }
}