using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;

using ClownCrew.GitBitch.Client.Exceptions;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Agents
{
    public sealed class ListenerAgent<T> : IDisposable
    {
        private readonly IEventHub _eventHub;
        private readonly SpeechRecognitionEngine _speechRecognitionEngine;
        private bool _listenerActice;

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

            _speechRecognitionEngine.LoadGrammar(gr);
            _speechRecognitionEngine.RequestRecognizerUpdate();

            try
            {
                _speechRecognitionEngine.SetInputToDefaultAudioDevice();
            }
            catch (InvalidOperationException exception)
            {
                throw new NoDefaultAudioDeviceException(exception);
            }

            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        public void StartListening()
        {
            if (_listenerActice) return;

            _listenerActice = true;
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, ListeningAudioState.Listening);
        }

        public void StopListening()
        {
            if (!_listenerActice) return;

            _listenerActice = false;
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, ListeningAudioState.NotListening);
        }

        public void Dispose()
        {
            _speechRecognitionEngine.RecognizeAsyncStop();
            _speechRecognitionEngine.Dispose();
            StopListening();
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
            //System.Diagnostics.Debug.WriteLine("Load grammar completed.");
        }

        private void EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Emulate recognize completed '" + e.Result.Text + "'.");
        }

        private void AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Audio state changed to " + e.AudioState + ".");
        }

        private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (!_listenerActice) return;

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
            if (!_listenerActice) return;

            System.Diagnostics.Debug.WriteLine("Speech recognized as '" + e.Result.Text + "'.");
            InvokeHeardSomethingEvent(Source.ListenerAgent, e.Result.Text);
        }
    }
}