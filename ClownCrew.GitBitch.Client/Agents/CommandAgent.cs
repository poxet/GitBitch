using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class CommandAgent : ICommandAgent
    {
        private static readonly object SyncRoot = new object();
        private readonly ITalkAgent _talkAgent;
        private readonly List<IGitBitchCommand> _commands;
        private readonly SpeechRecognitionEngine _sre;
        private bool _initiated;

        public CommandAgent(ITalkAgent talkAgent)
        {
            _talkAgent = talkAgent;
            _commands = new List<IGitBitchCommand>();

            ////TODO: Start to listen for commands
            //var choices = new Choices();
            //choices.Add("help");
            ////choices.Add(alternatives.SelectMany(x => x.Phrases).ToArray());
            //var gr = new Grammar(new GrammarBuilder(choices));

            _sre = new SpeechRecognitionEngine();
            //try
            //{
            //    _sre.RequestRecognizerUpdate();
            //    _sre.LoadGrammar(gr);
                _sre.SpeechRecognized += SpeechRecognized;
            //    //sre.SpeechDetected += localSR_SpeechDetected;
            //    //sre.SpeechRecognitionRejected += localSR_SpeechRecognitionRejected;
            //    //sre.SpeechHypothesized += localSR_SpeechHypothesized;
            //    //sre.AudioStateChanged += localSR_AudioStateChanged;
            //    //sre.EmulateRecognizeCompleted += localSR_EmulateRecognizeCompleted;
            //    //sre.LoadGrammarCompleted += localSR_LoadGrammarCompleted;
            //    //sre.RecognizeCompleted += localSR_RecognizeCompleted;
            //    //sre.RecognizerUpdateReached += localSR_RecognizerUpdateReached;
            //    //sre.AudioLevelUpdated += localSR_AudioLevelUpdated;
            //    //sre.AudioSignalProblemOccurred += localSR_AudioSignalProblemOccurred;
                _sre.SetInputToDefaultAudioDevice();
            //    _sre.RecognizeAsync(RecognizeMode.Multiple);

            //    //var actualPhrase = await _talkAgent.SayAsync(question);

            //    ////TODO: Wait for input, pick default if there is no response in a while
            //    //if (!_responseEvent.WaitOne(millisecondsTimeout))
            //    //{
            //    //    var defaultAlternative = alternatives.First(x => x.IsDefault);
            //    //    var response = "No answer, so " + defaultAlternative.Phrases.First() + " then.";
            //    //    await _talkAgent.SayAsync(response);
            //    //    return new Answer<T>(defaultAlternative.Response);
            //    //}

            //    ////TODO: Get the selected alternative here
            //    //throw new NotImplementedException();
            //    //return new Tuple<string, T>(actualPhrase, alternatives.Last().Item2);
            //}
            //finally
            //{
            //    //sre.RecognizeAsyncStop();
            //}
        }

        public IEnumerable<IGitBitchCommand> Commands { get { return _commands; } }

        private async void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            var command = FindCommand(e);
            if (command != null)
            {
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
                }
            }
        }

        private void Command_RegisterPhraseEvent(object sender, RegisterPhraseEventArgs e)
        {
            AddPhrases(e.Phrases);
        }
    }
}