using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Agents
{
    public sealed class ListenerAgent<T> : IDisposable
    {
        private readonly IEventHub _eventHub;
        private readonly SpeechRecognitionEngine _speechRecognitionEngine;

        public event EventHandler<HeardSomethingEventArgs> HeardSomethingEvent;

        private void InvokeHeardSomethingEvent(Source source, string phrase)
        {
            var handler = HeardSomethingEvent;
            if (handler != null)
                handler(this, new HeardSomethingEventArgs(source, phrase));

            _eventHub.InvokeHeardSomethingEvent(source, phrase);
        }

        public ListenerAgent(IEventHub eventHub, IEnumerable<QuestionAnswerAlternative<T>> alternatives = null)
        {
            _eventHub = eventHub;
            Grammar gr;
            if (alternatives != null)
            {
                var choices = new Choices();
                choices.Add(alternatives.SelectMany(x => x.Phrases).ToArray());
                gr = new Grammar(new GrammarBuilder(choices));
            }
            else
            {
                gr = new DictationGrammar();
            }

            _speechRecognitionEngine = new SpeechRecognitionEngine();
            _speechRecognitionEngine.LoadGrammar(gr);

            _speechRecognitionEngine.RequestRecognizerUpdate();
            _speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            _speechRecognitionEngine.SpeechDetected += SpeechDetected;
            _speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionRejected;
            _speechRecognitionEngine.SpeechHypothesized += SpeechHypothesized;
            _speechRecognitionEngine.AudioStateChanged += AudioStateChanged;
            _speechRecognitionEngine.EmulateRecognizeCompleted += EmulateRecognizeCompleted;
            _speechRecognitionEngine.LoadGrammarCompleted += LoadGrammarCompleted;
            _speechRecognitionEngine.RecognizeCompleted += RecognizeCompleted;
            _speechRecognitionEngine.RecognizerUpdateReached += RecognizerUpdateReached;
            _speechRecognitionEngine.AudioLevelUpdated += AudioLevelUpdated;
            _speechRecognitionEngine.AudioSignalProblemOccurred += AudioSignalProblemOccurred;
            _speechRecognitionEngine.SetInputToDefaultAudioDevice();
        }

        public void StartListening()
        {
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        public void Dispose()
        {
            _speechRecognitionEngine.RecognizeAsyncStop();
            _speechRecognitionEngine.Dispose();
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, AudioState.Stopped);
        }

        private void AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            _eventHub.InvokeAudioInputLevelChangedEvent(Source.ListenerAgent, e.AudioLevel);
        }

        private void RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Recognizer update reached.");
        }

        private void RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Recognize completed. " + (e.Cancelled ? "Cancelled" : string.Empty) + ".");
        }

        private void LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Audio state changed to " + e.AudioState + ".");
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, e.AudioState);
        }

        private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result.Text != string.Empty)
            {
                System.Diagnostics.Debug.WriteLine("Speech recognition rejected '" + e.Result.Text + "'.");
                InvokeHeardSomethingEvent(Source.ListenerAgent, e.Result.Text);
            }
        }

        private void SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech detected with audio position " + e.AudioPosition + ".");
        }

        private void AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Audio signal problem occurred.");
        }

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech recognized as '" + e.Result.Text + "'.");
            InvokeHeardSomethingEvent(Source.ListenerAgent, e.Result.Text);
        }
    }
}