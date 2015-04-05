using System.ComponentModel;
using System.Runtime.CompilerServices;
using ClownCrew.GitBitch.Client.Annotations;

namespace ClownCrew.GitBitch.Client.ViewModels
{
    internal sealed class EnterStringViewModel : INotifyPropertyChanged
    {
        public EnterStringViewModel()
        {
            Title = "Title...";
            Label = "Question...";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public string Title { get; private set; }
        public string Label { get; private set; }
        public string StringValue { get; set; }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}