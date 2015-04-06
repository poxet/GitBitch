using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Model.EventArgs
{
    public class RepositoryAddedEventArgs : System.EventArgs
    {
        private readonly IGitRepository _gitRepository;

        public RepositoryAddedEventArgs(IGitRepository gitRepository)
        {
            _gitRepository = gitRepository;
        }

        public IGitRepository GitRepository { get { return _gitRepository; } }
    }
}