namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class AudioInputStateChangedEventArgs
    {
        private readonly Source _source;
        private readonly ListeningAudioState _listeningAudioState;

        public AudioInputStateChangedEventArgs(Source source, ListeningAudioState listeningAudioState)
        {
            _source = source;
            _listeningAudioState = listeningAudioState;
        }

        public Source Source { get { return _source; } }
        public ListeningAudioState ListeningAudioState { get { return _listeningAudioState; } }
    }
}