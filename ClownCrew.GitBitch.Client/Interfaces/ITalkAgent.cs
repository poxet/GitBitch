﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ITalkAgent
    {
        Task<string> SayAsync(string phrase);
        Task<Answer<T>> AskAsync<T>(string question, List<QuestionAnswerAlternative<T>> alternatives, int millisecondsTimeout = 3000);
    }
}