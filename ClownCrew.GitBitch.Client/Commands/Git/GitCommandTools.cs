using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;

namespace ClownCrew.GitBitch.Client.Commands.Git
{
    public class GitCommandTools
    {
        public static async Task<string> GetSelectedPathAsync(IRepositoryBusines repositoryBusiness, ITalkAgent talkAgent, ISettingAgent settingAgent)
        {
            var gitRepoPath = repositoryBusiness.GetSelectedPath();
            if (string.IsNullOrEmpty(gitRepoPath))
            {
                var repos = settingAgent.GetSettings<string>("Repositories");
                if (!repos.Any())
                    await talkAgent.SayAsync("You need to open a repository before you can ask for status.");
                else
                    await talkAgent.SayAsync("You need to select a repository before you can ask for status.");
            }

            return gitRepoPath;
        }
    }
}