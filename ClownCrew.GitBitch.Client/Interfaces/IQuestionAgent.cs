using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IQuestionAgent
    {
        Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000);
        Task<bool> AskYesNoAsync(string question, int millisecondsTimeout = 3000);
    }
}