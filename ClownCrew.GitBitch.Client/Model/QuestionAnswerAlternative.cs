using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Model
{
    public class QuestionAnswerAlternative<T>
    {
        public List<string> Phrases { get; set; }
        public T Response { get; set; }
        public bool IsDefault { get; set; }
    }
}