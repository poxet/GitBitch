using System.Threading.Tasks;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IGitRepoAgent
    {
        Task SearchAsync();
    }
}