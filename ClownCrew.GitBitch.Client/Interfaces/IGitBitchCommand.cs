using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IGitBitchCommand
    {
        event EventHandler<RegisterPhraseEventArgs> RegisterPhraseEvent;

        string Name { get; }
        IEnumerable<string> Phrases { get; }
        Task ExecuteAsync(string key);
        string GetKey(string phrase);
    }
}