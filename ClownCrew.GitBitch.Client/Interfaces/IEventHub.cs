using System;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Interfaces
{
    public interface IEventHub
    {
        event EventHandler<AudioInputLevelChangedEventArgs> AudioInputLevelChangedEvent;
        event EventHandler<AudioInputStateChangedEventArgs> AudioInputStateChangedEvent;
        event EventHandler<HeardSomethingEventArgs> HeardSomethingEvent;
        event EventHandler<StartTalkingEventArgs> StartTalkingEvent;
        event EventHandler<DoneTalkingEventArgs> DoneTalkingEvent;
        event EventHandler<StartListeningEventArgs> StartListeningEvent;
        event EventHandler<DoneListeningEventArgs> DoneListeningEvent;
        void InvokeAudioInputLevelChangedEvent(Source source, int audioLevel);
        void InvokeAudioInputStateChangedEvent(Source source, ListeningAudioState listeningAudioState);
        void InvokeHeardSomethingEvent(Source source, string phrase);
        void InvokeStartTalkingEvent(Guid talkId, string bitchName, string phrase);
        void InvokeDoneTalkingEvent(Guid talkId);
        void InvokeStartListeningEvent(Guid listenId);
        void InvokeDoneListeningEvent(Guid listenId);
    }
}