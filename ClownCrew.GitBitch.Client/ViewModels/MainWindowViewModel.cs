using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;
using ClownCrew.GitBitch.Client.Properties;

namespace ClownCrew.GitBitch.Client.ViewModels
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private int _audioInputLevel;
        private int _maxAudioInputLevel = 10;
        private string _listeningStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Phrases = new SafeObservableCollection<string>();

            CompositeRoot.Instance.EventHub.StartTalkingEvent += EventHub_StartTalkingEvent;
            CompositeRoot.Instance.EventHub.DoneTalkingEvent += EventHub_DoneTalkingEvent;
            CompositeRoot.Instance.EventHub.AudioInputLevelChangedEvent += CommandAgentAudioInputLevelChangedEvent;
            CompositeRoot.Instance.EventHub.AudioInputStateChangedEvent += EventHub_AudioInputStateChangedEvent;
            CompositeRoot.Instance.EventHub.HeardSomethingEvent += EventHub_HeardSomethingEvent;
        }

        void EventHub_DoneTalkingEvent(object sender, DoneTalkingEventArgs e)
        {
            //TODO: Change the color of stuff that was already said
        }

        private void EventHub_StartTalkingEvent(object sender, StartTalkingEventArgs e)
        {
            Phrases.Add(e.Name + ": " + e.Phrase);
        }

        private void EventHub_HeardSomethingEvent(object sender, HeardSomethingEventArgs e)
        {
            Phrases.Add("You: " + e.Phrase);
        }

        private void EventHub_AudioInputStateChangedEvent(object sender, AudioInputStateChangedEventArgs e)
        {
            ListeningStatus = e.Source + " is " + e.ListeningAudioState;
            if (e.ListeningAudioState == ListeningAudioState.NotListening) AudioInputLevel = 0;
        }

        private void CommandAgentAudioInputLevelChangedEvent(object sender, AudioInputLevelChangedEventArgs e)
        {
            AudioInputLevel = e.AudioLevel;            
        }

        public SafeObservableCollection<string> Phrases { get; private set; }

        public int AudioInputLevel
        {
            get { return _audioInputLevel; }
            set
            {
                if (_audioInputLevel > MaxAudioInputLevel) MaxAudioInputLevel = _audioInputLevel;
                _audioInputLevel = value;
                OnPropertyChanged();
            }
        }

        public int MaxAudioInputLevel
        {
            get { return _maxAudioInputLevel; }
            set
            {
                _maxAudioInputLevel = value;
                OnPropertyChanged();
            }
        }

        public string ListeningStatus
        {
            get { return _listeningStatus; }
            set
            {
                _listeningStatus = value;
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