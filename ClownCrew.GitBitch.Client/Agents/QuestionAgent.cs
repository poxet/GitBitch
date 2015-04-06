using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.ViewModels;
using ClownCrew.GitBitch.Client.Views;

namespace ClownCrew.GitBitch.Client.Agents
{
    public class QuestionAgent : IQuestionAgent
    {
        private readonly AutoResetEvent _responseEvent = new AutoResetEvent(false);
        private readonly ITalkAgent _talkAgent;
        private readonly ISettingAgent _settingAgent;
        private string _responsePhrase;

        public QuestionAgent(ITalkAgent talkAgent, ISettingAgent settingAgent)
        {
            _talkAgent = talkAgent;
            _settingAgent = settingAgent;
        }

        public async Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000)
        {
            using (var listenerAgent = new ListenerAgent<T>(alternatives))
            {
                listenerAgent.HeardSomethingEvent += ListenerAgent_HeardSomethingEvent;

                ListenerAgent.OnStartEvent();

                await _talkAgent.SayAsync(question);
                listenerAgent.StartListening();

                string defaultResponse = null;
                var r = await Task.Factory.StartNew(() =>
                {
                    if (!_responseEvent.WaitOne(millisecondsTimeout))
                    {
                        var defaultAlternative = alternatives.FirstOrDefault(x => x.IsDefault) ?? alternatives.First();
                        //defaultResponse = "No answer, so " + defaultAlternative.Phrases.First() + " then.";
                        return new Answer<T>(defaultAlternative.Response);
                    }

                    var selectedAlternative = alternatives.First(x => x.Phrases.Any(y => y == _responsePhrase));
                    return new Answer<T>(selectedAlternative.Response);
                });

                if (!string.IsNullOrEmpty(defaultResponse))
                    await _talkAgent.SayAsync(defaultResponse);

                ListenerAgent.OnEndEvent();

                return r;
            }
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

        public async Task<string> AskFolderAsync(string question, int millisecondsTimeout = 3000)
        {
            var task = _talkAgent.SayAsync(question);

            var strFriendlyName = AppDomain.CurrentDomain.FriendlyName;
            var pro = Process.GetProcessesByName(strFriendlyName.Substring(0, strFriendlyName.LastIndexOf('.')));

            string response = null;

            var dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = false;
            dlg.SelectedPath = _settingAgent.GetSetting<string>("LastPath", null);
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

            if (response ?? false) resp = ((EnterStringViewModel) enterStringWindow.DataContext).StringValue;
            await task;

            return resp;
        }

        private void ListenerAgent_HeardSomethingEvent(object sender, HeardSomethingEventArgs e)
        {
            _responsePhrase = e.Phrase;
            _responseEvent.Set();
        }        
    }
}