using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;
using ClownCrew.GitBitch.Client.ViewModels;
using ClownCrew.GitBitch.Client.Views;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class QuestionAgent : IQuestionAgent
    {
        private readonly AutoResetEvent _responseEvent = new AutoResetEvent(false);
        private readonly ITalkAgent _talkAgent;
        private readonly ISettingAgent _settingAgent;
        private readonly IEventHub _eventHub;
        private string _responsePhrase;

        public QuestionAgent(ITalkAgent talkAgent, ISettingAgent settingAgent, IEventHub eventHub)
        {
            _talkAgent = talkAgent;
            _settingAgent = settingAgent;
            _eventHub = eventHub;
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            if (_settingAgent.GetSetting("Muted", false))
            {
                //TODO: Show dialog where the anser can be entered. The dialog should have the same timeout as the default answer.
                //There should be one answer for each alternative, up to a limit, where there will be a drop down.
                //If there are no set alternatives, there should be a text input box for answers.
                //Also look at the other question functions: AskYesNoAsync, AskFolderAsync and AskStringAsync. And align them as well.
                return GetDefaultAnswer(alternatives);
            }

            try
            {
                using (var listenerAgent = new ListenerAgent<T>(_eventHub, alternatives))
                {
                    listenerAgent.HeardSomethingEvent += EventHub_HeardSomethingEvent;

                    var listenId = Guid.NewGuid();
                    _eventHub.InvokeStartListeningEvent(listenId);

                    await _talkAgent.SayAsync(question);
                    listenerAgent.StartListening();

                    var r = await Task.Factory.StartNew(() =>
                        {
                            if (!_responseEvent.WaitOne(millisecondsTimeout))
                            {
                                return GetDefaultAnswer(alternatives);
                            }

                            var selectedAlternative = alternatives.First(x => x.Phrases.Any(y => y == _responsePhrase));
                            return new Answer<T>(selectedAlternative.Response);
                        });

                    _eventHub.InvokeDoneListeningEvent(listenId);

                    return r;
                }
            }
            catch (InvalidOperationException exception)
            {
                CompositeRoot.Instance.TalkAgent.SayAsync("Oups, now we have problems! " + exception.Message);
                _settingAgent.SetSetting("Muted", true);
            }

            return GetDefaultAnswer(alternatives);
        }

        private static Answer<T> GetDefaultAnswer<T>(List<QuestionAnswerAlternative<T>> alternatives)
        {
            var defaultAlternative = alternatives.FirstOrDefault(x => x.IsDefault) ?? alternatives.First();
            var answer = new Answer<T>(defaultAlternative.Response);
            return answer;
        }

        private void EventHub_HeardSomethingEvent(object sender, HeardSomethingEventArgs e)
        {
            _responsePhrase = e.Phrase;
            _responseEvent.Set();
        }

        public async Task<bool> AskYesNoAsync(string question, int millisecondsTimeout = 3000)
        {
            var questionAnswerAlternatives = new List<QuestionAnswerAlternative<bool>>
            {
                new QuestionAnswerAlternative<bool>
                {
                    Phrases = new List<string> { "Yes", "Sure" },
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

        //[STAThread]
        public async Task<string> AskFolderAsync(string question, int millisecondsTimeout = 3000)
        {
            //System.Windows.Threading.Dispatcher dispatcher = (System.Windows.Threading.Dispatcher)d;

            var task = _talkAgent.SayAsync(question);

            //var strFriendlyName = AppDomain.CurrentDomain.FriendlyName;
            //var pro = Process.GetProcessesByName(strFriendlyName.Substring(0, strFriendlyName.LastIndexOf('.')));

            string response = null;

            var dlg = new FolderBrowserDialog { ShowNewFolderButton = false, SelectedPath = _settingAgent.GetSetting<string>("LastPath", null) };
            //var dlg2 = new EnterStringWindow();

            //if (Dispatcher.CurrentDispatcher.Thread.IsBackground)
            //{
            //    return await Dispatcher.CurrentDispatcher.Invoke(() => AskFolderAsync(question, millisecondsTimeout));
            //}

            //Invoke((Action)(() => { saveFileDialog.ShowDialog() }));            

            //var windowWrapper = new WindowWrapper(pro[0].MainWindowHandle);
            var windowWrapper = new ForegroundWindow();
            if (dlg.ShowDialog(windowWrapper) == DialogResult.OK)
            {
                _settingAgent.SetSetting("LastPath", dlg.SelectedPath);
                response = dlg.SelectedPath;
            }

            await task;
            return response;
        }

        public async Task<string> AskStringAsync(string question, int millisecondsTimeout = 3000)
        {
            string resp = null;
            var task = _talkAgent.SayAsync(question);

            var enterStringWindow = new EnterStringWindow { Topmost = true };
            var response = enterStringWindow.ShowDialog();

            if (response ?? false) resp = ((EnterStringViewModel)enterStringWindow.DataContext).StringValue;
            await task;

            return resp;
        }
    }
}