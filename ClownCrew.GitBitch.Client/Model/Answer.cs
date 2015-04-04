namespace ClownCrew.GitBitch.Client.Model
{
    public class Answer<T>
    {
        public Answer(T response)
        {
            Response = response;
        }

        public T Response { get; private set; }
    }
}