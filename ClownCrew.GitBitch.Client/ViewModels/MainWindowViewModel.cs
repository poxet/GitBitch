using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Annotations;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.ViewModels
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Phrases = new SafeObservableCollection<string>();

            CompositeRoot.Instance.TalkAgent.StartSayEvent += TalkAgentStartSayEvent;
            CompositeRoot.Instance.TalkAgent.SayCompleteEvent += TalkAgent_SayCompleteEvent;
            ListenerAgent.HeardEvent += MainWindowViewModel_HeardEvent;
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

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}