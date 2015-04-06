namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class HeardSomethingEventArgs
    {
        private readonly Source _source;
        private readonly string _phrase;

        public HeardSomethingEventArgs(Source source, string phrase)
        {
            _source = source;
            _phrase = phrase;
        }

        public Source Source { get { return _source; } }
        public string Phrase { get { return _phrase; } }
    }
}