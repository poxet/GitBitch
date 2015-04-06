using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Model
{
    public class GitRepository : IGitRepository
    {
        private readonly string _name;
        private readonly string _path;

        public GitRepository(string name, string path)
        {
            _name = name;
            _path = path;
        }

        public string Name { get { return _name; } }
        public string Path { get { return _path; } }
    }
}