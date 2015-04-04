using System;
using System.Collections.Generic;
using ClownCrew.GitBitch.Client.Agents;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IGitBitchCommand
    {
        event EventHandler<RegisterPhraseEventArgs> RegisterPhraseEvent;

        IEnumerable<string> Phrases { get; }
        void Execute();
    }
}