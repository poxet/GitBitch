using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClownCrew.GitBitch.Client.Annotations;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.ViewModels
{
    internal sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            Phrases = new ObservableCollection<string>();

            CompositeRoot.Instance.TalkAgent.SayEvent += TalkAgent_SayEvent;
        }

        private void TalkAgent_SayEvent(object sender, SayEventArgs e)
        {
            Phrases.Add(e.Phrase);
        }

        public ObservableCollection<string> Phrases { get; private set; }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}