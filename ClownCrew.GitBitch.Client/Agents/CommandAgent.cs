using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Threading.Tasks;
using Castle.Core.Internal;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandAgent : ICommandAgent
    {
        public event EventHandler<CommandStateChangedEventArgs> CommandStateChangedEvent;

        protected virtual void OnCommandStateChangedEvent(string status)
        {
            var handler = CommandStateChangedEvent;
            if (handler != null) handler(this, new CommandStateChangedEventArgs(status));
        }

        private int _interruptCounter = 0;
        private static readonly object SyncRoot = new object();
        private static readonly object SyncInterrupt = new object();
        private readonly ITalkAgent _talkAgent;
        private readonly List<IGitBitchCommand> _commands;
        private readonly SpeechRecognitionEngine _sre;
        private bool _initiated;

        public CommandAgent(ITalkAgent talkAgent)
        {
            talkAgent.StartSayEvent += talkAgent_StartSayEvent;
            talkAgent.SayCompleteEvent += talkAgent_SayCompleteEvent;
            ListenerAgent.EndEvent += ListenerAgent_EndEvent;
            ListenerAgent.StartEvent += ListenerAgent_StartEvent;

            _talkAgent = talkAgent;
            _commands = new List<IGitBitchCommand>();

            _sre = new SpeechRecognitionEngine();
            _sre.SpeechRecognized += SpeechRecognized;
            _sre.SpeechDetected += _sre_SpeechDetected;
            _sre.SpeechRecognitionRejected += _sre_SpeechRecognitionRejected;
            _sre.SpeechHypothesized += _sre_SpeechHypothesized;
            _sre.AudioStateChanged += _sre_AudioStateChanged;
            _sre.EmulateRecognizeCompleted += _sre_EmulateRecognizeCompleted;
            _sre.LoadGrammarCompleted += _sre_LoadGrammarCompleted;
            _sre.RecognizeCompleted += _sre_RecognizeCompleted;
            _sre.RecognizerUpdateReached += _sre_RecognizerUpdateReached;
            _sre.AudioLevelUpdated += _sre_AudioLevelUpdated;
            _sre.AudioSignalProblemOccurred += _sre_AudioSignalProblemOccurred;
            _sre.SetInputToDefaultAudioDevice();
        }

        void ListenerAgent_StartEvent(object sender, EventArgs e)
        {
            Pause();
        }

        void ListenerAgent_EndEvent(object sender, EventArgs e)
        {
            Resume();
        }

        void talkAgent_SayCompleteEvent(object sender, SayCompleteEventArgs e)
        {
            Resume();
        }

        private void Resume()
        {
            lock (SyncInterrupt)
            {
                _interruptCounter--;
                if (_interruptCounter == 0)
                    _sre.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        void talkAgent_StartSayEvent(object sender, StartSayEventArgs e)
        {
            Pause();
        }

        private void Pause()
        {
            lock (SyncInterrupt)
            {
                if (_interruptCounter == 0)
                    _sre.RecognizeAsyncStop();
                _interruptCounter++;
            }
        }

        void _sre_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Debug.WriteLine("Speech detected: " + e.AudioPosition + ".");
        }

        void _sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine("Speech recognition rejected '" + e.Result.Text + "'.");
        }

        void _sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        void _sre_EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            Debug.WriteLine("Emulate recognize completed '" + e.Result.Text + "'.");
        }

        void _sre_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Result != null)
                Debug.WriteLine("Recognize completed '" + e.Result.Text + "'.");
        }

        void _sre_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            //Debug.WriteLine("Load grammar completed: " + e.Grammar.Name);
        }

        void _sre_AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            Debug.WriteLine("Audio signal problem occurred: " + e.AudioSignalProblem);
        }

        void _sre_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            Debug.WriteLine("Recognizer update reached: " + e.AudioPosition + " " + e.UserToken);
        }

        void _sre_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            //TODO: Send theese events so that a "sound strength bar" can e displayed in the status bar.
            //Debug.WriteLine("Audio Level Updated: " + e.AudioLevel);
        }

        void _sre_AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            Debug.WriteLine("Audio state changed: " + e.AudioState);
            OnCommandStateChangedEvent("Audio state changed: " + e.AudioState);
        }

        public IEnumerable<IGitBitchCommand> Commands { get { return _commands; } }

        private async void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var command = FindCommand(e);
            if (command != null)
            {
                ListenerAgent.InvokeHeardEvent(e.Result.Text);
                await command.ExecuteAsync(command.GetKey(e.Result.Text));
            }
        }

        private IGitBitchCommand FindCommand(SpeechRecognizedEventArgs e)
        {
            return _commands.FirstOrDefault(command => command.Phrases.Any(phrase => string.Compare(phrase, e.Result.Text, StringComparison.InvariantCultureIgnoreCase) == 0));
        }

        public async Task ClrearAsync()
        {
            _sre.RecognizeAsyncStop();
            _commands.Clear();
            lock (SyncRoot)
            {
                _sre.UnloadAllGrammars();
                _initiated = false;
            }
        }

        public async Task RegisterAsync(IGitBitchCommands gitBitchCommands)
        {
            foreach (var command in gitBitchCommands.Items)
            {
                if (command.Phrases.Any())
                {
                    AddPhrases(command.Phrases.ToArray());
                }

                command.RegisterPhraseEvent += Command_RegisterPhraseEvent;
                _commands.Add(command);
            }
        }

        private async void AddPhrases(IEnumerable<string> phrases)
        {
            var choices = new Choices();
            choices.Add(phrases.ToArray());
            
            lock (SyncRoot)
            {
                _sre.LoadGrammarAsync(new Grammar(choices));

                if (!_initiated)
                {
                    _initiated = true;
                    _sre.RecognizeAsync(RecognizeMode.Multiple);
                    //Thread.Sleep(1000);
                    //_sre.RecognizeAsyncStop();
                    //Thread.Sleep(1000);
                    //_sre.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        private void Command_RegisterPhraseEvent(object sender, RegisterPhraseEventArgs e)
        {
            AddPhrases(e.Phrases);
        }
    }
}