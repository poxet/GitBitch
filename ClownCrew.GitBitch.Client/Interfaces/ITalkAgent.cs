using System;
using System.Threading.Tasks;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface ITalkAgent
    {
        event EventHandler<SayEventArgs> SayEvent;

        Task<string> SayAsync(string phrase);
    }
}