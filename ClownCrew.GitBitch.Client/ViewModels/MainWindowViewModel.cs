using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Annotations;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.ViewModels
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _mainCommandListener;
        private string _questionCommandListener;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Phrases = new SafeObservableCollection<string>();

            CompositeRoot.Instance.TalkAgent.StartSayEvent += TalkAgentStartSayEvent;
            CompositeRoot.Instance.TalkAgent.SayCompleteEvent += TalkAgent_SayCompleteEvent;
            ListenerAgent.HeardEvent += MainWindowViewModel_HeardEvent;

            CompositeRoot.Instance.CommandAgent.CommandStateChangedEvent += CommandAgent_CommandStateChangedEvent;
            ListenerAgent.StateChangedEvent += ListenerAgent_StateChangedEvent;
        }

        void ListenerAgent_StateChangedEvent(object sender, CommandStateChangedEventArgs e)
        {
            QuestionCommandListener = e.Status;
        }

        void CommandAgent_CommandStateChangedEvent(object sender, CommandStateChangedEventArgs e)
        {
            MainCommandListener = e.Status;
        }

        void MainWindowViewModel_HeardEvent(object sender, HeardEventArgs e)
        {
            Phrases.Add("You: " + e.Phrase);
        }

        private void TalkAgent_SayCompleteEvent(object sender, SayCompleteEventArgs e)
        {
        }

        private void TalkAgentStartSayEvent(object sender, StartSayEventArgs e)
        {
            Phrases.Add(e.Name + ": " + e.Phrase);
        }

        public SafeObservableCollection<string> Phrases { get; private set; }

        public string QuestionCommandListener
        {
            get { return _questionCommandListener; }
            set
            {
                _questionCommandListener = value;
                OnPropertyChanged();
            }
        }

        public string MainCommandListener
        {
            get { return _mainCommandListener; }
            set
            {
                _mainCommandListener = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}