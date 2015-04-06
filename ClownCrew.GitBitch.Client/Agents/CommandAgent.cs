using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Speech.Recognition;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandAgent : ICommandAgent
    {
        private static readonly object SyncRoot = new object();
        private readonly IEventHub _eventHub;
        private readonly List<IGitBitchCommand> _commands;
        private readonly SpeechRecognitionEngine _speechRecognitionEngine;
        private bool _initiated;
        private int _interruptCounter;
        private bool _listenerActice;

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

            _eventHub.InvokeAudioInputStateChangedEvent(Source.CommandAgent, ListeningAudioState.NotListening);
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
            lock (SyncRoot)
            {
                _interruptCounter--;
                if (_interruptCounter == 0)
                {
                    StartListening();
                }
            }
        }

        private void PauseListening()
        {
            lock (SyncRoot)
            {
                if (_interruptCounter == 0)
                {
                    StopListening();
                }

                _interruptCounter++;
            }
        }

        private void StartListening()
        {
            if (_listenerActice) return;

            _listenerActice = true;
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, ListeningAudioState.Listening);
        }

        private void StopListening()
        {
            if (!_listenerActice) return;

            _listenerActice = false;
            _eventHub.InvokeAudioInputStateChangedEvent(Source.ListenerAgent, ListeningAudioState.NotListening);
        }

        private void SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Debug.WriteLine("Speech detected: " + e.AudioPosition + ".");
        }

        private void SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine("Speech recognition rejected '" + e.Result.Text + "'.");
        }

        private void SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine("Speech hypothesized '" + e.Result.Text + "'.");
        }

        private void EmulateRecognizeCompleted(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            Debug.WriteLine("Emulate recognize completed '" + e.Result.Text + "'.");
        }

        private void RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            if (e.Result != null) Debug.WriteLine("Recognize completed '" + e.Result.Text + "'.");
        }

        private void LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            Debug.WriteLine("Load grammar completed: " + e.Grammar.Name);
        }

        private void AudioSignalProblemOccurred(object sender, AudioSignalProblemOccurredEventArgs e)
        {
            Debug.WriteLine("Audio signal problem occurred: " + e.AudioSignalProblem);
        }

        private void RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            Debug.WriteLine("Recognizer update reached: " + e.AudioPosition + " " + e.UserToken);
        }

        private void AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            _eventHub.InvokeAudioInputLevelChangedEvent(Source.CommandAgent, e.AudioLevel);
        }

        private void AudioStateChanged(object sender, AudioStateChangedEventArgs e)
        {
            Debug.WriteLine("Audio state changed: " + e.AudioState);
        }

        public IEnumerable<IGitBitchCommand> Commands { get { return _commands; } }

        private async void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (!_listenerActice) return;

            var command = FindCommand(e);
            if (command != null)
            {
                _eventHub.InvokeHeardSomethingEvent(Source.CommandAgent, e.Result.Text);
                await command.ExecuteAsync(command.GetKey(e.Result.Text), e.Result.Text);
            }
        }

        private IGitBitchCommand FindCommand(SpeechRecognizedEventArgs e)
        {
            return _commands.FirstOrDefault(command => command.Phrases.Any(phrase => string.Compare(phrase, e.Result.Text, StringComparison.InvariantCultureIgnoreCase) == 0));
        }

        public void ClearCommands()
        {
            _commands.Clear();
            _speechRecognitionEngine.UnloadAllGrammars();
        }

        public void RegisterCommands(IGitBitchCommands gitBitchCommands)
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

        private void AddPhrases(IEnumerable<string> phrases)
        {
            var choices = new Choices();
            choices.Add(phrases.ToArray());

            _speechRecognitionEngine.LoadGrammarAsync(new Grammar(choices));

            if (_initiated) return;

            lock (SyncRoot)
            {
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