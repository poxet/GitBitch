namespace ClownCrew.GitBitch.Client.Agents
{
    public class HeardSomethingEventArgs
    {
        private readonly string _phrase;

        public HeardSomethingEventArgs(string phrase)
        {
            _phrase = phrase;
        }

        public string Phrase { get { return _phrase; } }
    }
}