using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandAgent : ICommandAgent
    {
        private static readonly object SyncInitiated = new object();
        private static readonly object SyncInterrupt = new object();
        private readonly IEventHub _eventHub;
        private readonly List<IGitBitchCommand> _commands;
        private readonly SpeechRecognitionEngine _speechRecognitionEngine;
        private bool _initiated;
        private int _interruptCounter;

        public CommandAgent(IEventHub eventHub)
        {
            _eventHub = eventHub;
            _commands = new List<IGitBitchCommand>();

            _eventHub.StartTalkingEvent += EventHub_StartTalkingEvent;
            _eventHub.DoneTalkingEvent += EventHub_DoneTalkingEvent;
            _eventHub.StartListeningEvent += EventHub_StartListeningEvent;
            _eventHub.DoneListeningEvent += EventHub_DoneListeningEvent;

            _speechRecognitionEngine = new SpeechRecognitionEngine();
            _speechRecognitionEngine.SpeechRecognized += SpeechRecognized;
            _speechRecognitionEngine.SpeechDetected += SpeechRecognitionEngineSpeechDetected;
            _speechRecognitionEngine.SpeechRecognitionRejected += SpeechRecognitionEngineSpeechRecognitionRejected;
            _speechRecognitionEngine.SpeechHypothesized += SpeechRecognitionEngineSpeechHypothesized;
            _speechRecognitionEngine.AudioStateChanged += SpeechRecognitionEngineAudioStateChanged;
            _speechRecognitionEngine.EmulateRecognizeCompleted += SpeechRecognitionEngineEmulateRecognizeCompleted;
            _speechRecognitionEngine.LoadGrammarCompleted += SpeechRecognitionEngineLoadGrammarCompleted;
            _speechRecognitionEngine.RecognizeCompleted += SpeechRecognitionEngineRecognizeCompleted;
            _speechRecognitionEngine.RecognizerUpdateReached += SpeechRecognitionEngineRecognizerUpdateReached;
            _speechRecognitionEngine.AudioLevelUpdated += SpeechRecognitionEngineAudioLevelUpdated;
            _speechRecognitionEngine.AudioSignalProblemOccurred += SpeechRecognitionEngineAudioSignalProblemOccurred;
            _speechRecognitionEngine.SetInputToDefaultAudioDevice();
        }

        private void EventHub_DoneListeningEvent(object sender, DoneListeningEventArgs e)
        {
            ResumeListening();
        }

        private void EventHub_StartListeningEvent(object sender, StartListeningEventArgs e)
        {
            PauseListening();
        }

        private void EventHub_DoneTalkingEvent(object sender, DoneTalkingEventArgs e)
        {
            ResumeListening();
        }

        private void EventHub_StartTalkingEvent(object sender, StartTalkingEventArgs e)
        {
            PauseListening();
        }

        private void ResumeListening()
        {
            lock (SyncInterrupt)
            {
                _interruptCounter--;
                if (_interruptCounter == 0)
                    _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
        }

        private void PauseListening()
        {
            lock (SyncInterrupt)
            {
                if (_interruptCounter == 0)
                    _speechRecognitionEngine.RecognizeAsyncStop();
                _interruptCounter++;
            }
        }

        private void SpeechRecognitionEngineSpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Debug.WriteLine("Speech detected: " + e.AudioPosition + ".");
        }

        private void SpeechRecognitionEngineSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine("Speech recognition rejected '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionEngineSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionEngineEmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            Debug.WriteLine("Emulate recognize completed '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionEngineRecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Result != null)
                Debug.WriteLine("Recognize completed '" + e.Result.Text + "'.");
        }

        private void SpeechRecognitionEngineLoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            //Debug.WriteLine("Load grammar completed: " + e.Grammar.Name);
        }

        private void SpeechRecognitionEngineAudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            Debug.WriteLine("Audio signal problem occurred: " + e.AudioSignalProblem);
        }

        private void SpeechRecognitionEngineRecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            Debug.WriteLine("Recognizer update reached: " + e.AudioPosition + " " + e.UserToken);
        }

        private void SpeechRecognitionEngineAudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            _eventHub.InvokeAudioInputLevelChangedEvent(Source.CommandAgent, e.AudioLevel);
        }

        private void SpeechRecognitionEngineAudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            _eventHub.InvokeAudioInputStateChangedEvent(Source.CommandAgent, e.AudioState);
        }

        public IEnumerable<IGitBitchCommand> Commands { get { return _commands; } }

        private async void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var command = FindCommand(e);
            if (command != null)
            {
                _eventHub.InvokeHeardSomethingEvent(Source.CommandAgent, e.Result.Text);
                await command.ExecuteAsync(command.GetKey(e.Result.Text));
            }
        }

        private IGitBitchCommand FindCommand(SpeechRecognizedEventArgs e)
        {
            return _commands.FirstOrDefault(command => command.Phrases.Any(phrase => string.Compare(phrase, e.Result.Text, StringComparison.InvariantCultureIgnoreCase) == 0));
        }

        public async Task ClrearAsync()
        {
            _speechRecognitionEngine.RecognizeAsyncStop();
            _commands.Clear();
            lock (SyncInitiated)
            {
                _speechRecognitionEngine.UnloadAllGrammars();
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
            
            lock (SyncInitiated)
            {
                _speechRecognitionEngine.LoadGrammarAsync(new Grammar(choices));

                if (!_initiated)
                {
                    _initiated = true;
                    _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
                }
            }
        }

        private void Command_RegisterPhraseEvent(object sender, RegisterPhraseEventArgs e)
        {
            AddPhrases(e.Phrases);
        }
    }
}