namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class RegisterPhraseEventArgs : System.EventArgs
    {
        private readonly string[] _phrases;

        public RegisterPhraseEventArgs(string[] phrases)
        {
            _phrases = phrases;
        }

        public string[] Phrases { get { return _phrases; } }
    }
}