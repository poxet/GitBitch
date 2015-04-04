using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Threading;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class QuestionAgent : IQuestionAgent
    {
        private readonly static AutoResetEvent _responseEvent = new AutoResetEvent(false);
        private readonly ITalkAgent _talkAgent;
        
        public QuestionAgent(ITalkAgent talkAgent)
        {
            _talkAgent = talkAgent;
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            var choices = new Choices();
            choices.Add(alternatives.SelectMany(x => x.Phrases).ToArray());
            var gr = new Grammar(new GrammarBuilder(choices));

            var sre = new SpeechRecognitionEngine();
            try
            {
                sre.RequestRecognizerUpdate();
                sre.LoadGrammar(gr);
                sre.SpeechRecognized += localSR_SpeechRecognized;
                sre.SpeechDetected += localSR_SpeechDetected;
                sre.SpeechRecognitionRejected += localSR_SpeechRecognitionRejected;
                sre.SpeechHypothesized += localSR_SpeechHypothesized;
                sre.AudioStateChanged += localSR_AudioStateChanged;
                sre.EmulateRecognizeCompleted += localSR_EmulateRecognizeCompleted;
                sre.LoadGrammarCompleted += localSR_LoadGrammarCompleted;
                sre.RecognizeCompleted += localSR_RecognizeCompleted;
                sre.RecognizerUpdateReached += localSR_RecognizerUpdateReached;
                sre.AudioLevelUpdated += localSR_AudioLevelUpdated;
                sre.AudioSignalProblemOccurred += localSR_AudioSignalProblemOccurred;
                sre.SetInputToDefaultAudioDevice();
                sre.RecognizeAsync(RecognizeMode.Single);

                var actualPhrase = await _talkAgent.SayAsync(question);

                //TODO: Wait for input, pick default if there is no response in a while
                if (!_responseEvent.WaitOne(millisecondsTimeout))
                {
                    var defaultAlternative = alternatives.First(x => x.IsDefault);
                    var response = "No answer, so " + defaultAlternative.Phrases.First() + " then.";
                    await _talkAgent.SayAsync(response);
                    return new Answer<T>(defaultAlternative.Response);
                }

                //TODO: Get the selected alternative here
                throw new NotImplementedException();
                //return new Tuple<string, T>(actualPhrase, alternatives.Last().Item2);
            }
            finally
            {
                sre.RecognizeAsyncStop();
            }
        }

        void localSR_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            //TODO: Show a sound input bar in the status bar.
            //System.Diagnostics.Debug.WriteLine("Audio level changed to " + e.AudioLevel + ".");
        }

        void localSR_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void localSR_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Recognize completed. " + (e.Cancelled ? "Cancelled" : string.Empty) + ".");
        }

        void localSR_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void localSR_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void localSR_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Audio state changed to " + e.AudioState + ".");
        }

        void localSR_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech hypothesized " + e.Result.Text + ".");
        }

        void localSR_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void localSR_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech detected with audio position " + e.AudioPosition + ".");
        }

        void localSR_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Audio signal problem occurred.");
        }

        void localSR_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}