using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class HeardEventArgs : EventArgs
    {
        private readonly string _phrase;

        public HeardEventArgs(string phrase)
        {
            _phrase = phrase;
        }

        public string Phrase { get { return _phrase; } }
    }

    public class ListenerAgent
    {
        public static event EventHandler<HeardEventArgs> HeardEvent;
        public static event EventHandler<CommandStateChangedEventArgs> StateChangedEvent;
        public static event EventHandler<EventArgs> StartEvent;
        public static event EventHandler<EventArgs> EndEvent;

        public static void OnEndEvent()
        {
            var handler = EndEvent;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        public static void OnStartEvent()
        {
            var handler = StartEvent;
            if (handler != null) handler(null, EventArgs.Empty);
        }

        public static void InvokeStateChangedEvent(string status)
        {
            EventHandler<CommandStateChangedEventArgs> handler = StateChangedEvent;
            if (handler != null) handler(null, new CommandStateChangedEventArgs(status));
        }

        public static void InvokeHeardEvent(string phrase)
        {
            var handler = HeardEvent;
            if (handler != null) handler(null, new HeardEventArgs(phrase));
        }
    }

    public class ListenerAgent<T> : IDisposable
    {
        public event EventHandler<HeardSomethingEventArgs> HeardSomethingEvent;
        
        protected virtual void OnHeardSomethingEvent(string phrase)
        {
            var handler = HeardSomethingEvent;
            if (handler != null) handler(this, new HeardSomethingEventArgs(phrase));
        }

        private readonly SpeechRecognitionEngine _sre;

        public ListenerAgent(IEnumerable<QuestionAnswerAlternative<T>> alternatives = null)
        {
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

            _sre = new SpeechRecognitionEngine();
            _sre.LoadGrammar(gr);

            _sre.RequestRecognizerUpdate();
            _sre.SpeechRecognized += localSR_SpeechRecognized;
            _sre.SpeechDetected += localSR_SpeechDetected;
            _sre.SpeechRecognitionRejected += localSR_SpeechRecognitionRejected;
            _sre.SpeechHypothesized += localSR_SpeechHypothesized;
            _sre.AudioStateChanged += localSR_AudioStateChanged;
            _sre.EmulateRecognizeCompleted += localSR_EmulateRecognizeCompleted;
            _sre.LoadGrammarCompleted += localSR_LoadGrammarCompleted;
            _sre.RecognizeCompleted += localSR_RecognizeCompleted;
            _sre.RecognizerUpdateReached += localSR_RecognizerUpdateReached;
            _sre.AudioLevelUpdated += localSR_AudioLevelUpdated;
            _sre.AudioSignalProblemOccurred += localSR_AudioSignalProblemOccurred;
            _sre.SetInputToDefaultAudioDevice();

            ListenerAgent.InvokeStateChangedEvent("Question Audio state changed: N/A");
        }

        public void StartListening()
        {
            _sre.RecognizeAsync(RecognizeMode.Single);
        }

        public void Dispose()
        {
            _sre.RecognizeAsyncStop();
            _sre.Dispose();
            ListenerAgent.InvokeStateChangedEvent("Question Audio state changed: N/A");
        }

        void localSR_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            //TODO: Show a sound input bar in the status bar.
            //System.Diagnostics.Debug.WriteLine("Audio level changed to " + e.AudioLevel + ".");
        }

        void localSR_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Recognizer update reached.");
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
            ListenerAgent.InvokeStateChangedEvent("Question Audio state changed: " + e.AudioState);
        }

        void localSR_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        private void localSR_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result.Text != "")
            {
                System.Diagnostics.Debug.WriteLine("Speech recognition rejected '" + e.Result.Text + "'.");
                ListenerAgent.InvokeHeardEvent(e.Result.Text);
            }
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
            System.Diagnostics.Debug.WriteLine("Speech recognized as '" + e.Result.Text + "'.");
            ListenerAgent.InvokeHeardEvent(e.Result.Text);
            OnHeardSomethingEvent(e.Result.Text);
        }
    }
}