namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class AudioInputLevelChangedEventArgs
    {
        private readonly Source _source;
        private readonly int _audioLevel;

        public AudioInputLevelChangedEventArgs(Source source, int audioLevel)
        {
            _source = source;
            _audioLevel = audioLevel;
        }

        public Source Source { get { return _source; } }
        public int AudioLevel { get { return _audioLevel; } }
    }
}