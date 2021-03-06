using System;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;
using ClownCrew.GitBitch.Client.Model.EventArgs;

namespace ClownCrew.GitBitch.Client.Business
{
    public class EventHub : IEventHub
    {
        public event EventHandler<AudioInputLevelChangedEventArgs> AudioInputLevelChangedEvent;
        public event EventHandler<AudioInputStateChangedEventArgs> AudioInputStateChangedEvent;
        public event EventHandler<HeardSomethingEventArgs> HeardSomethingEvent;
        public event EventHandler<StartTalkingEventArgs> StartTalkingEvent;
        public event EventHandler<DoneTalkingEventArgs> DoneTalkingEvent;
        public event EventHandler<StartListeningEventArgs> StartListeningEvent;
        public event EventHandler<DoneListeningEventArgs> DoneListeningEvent;
        public event EventHandler<StartWorkingEventArgs> StartWorkingEvent;
        public event EventHandler<DoneWorkingEventArgs> DoneWorkingEvent;

        public virtual void InvokeAudioInputLevelChangedEvent(Source source, int audioLevel)
        {
            var handler = AudioInputLevelChangedEvent;
            if (handler != null) handler(this, new AudioInputLevelChangedEventArgs(source, audioLevel));
        }

        public void InvokeAudioInputStateChangedEvent(Source source, ListeningAudioState listeningAudioState)
        {
            var handler = AudioInputStateChangedEvent;
            if (handler != null) handler(this, new AudioInputStateChangedEventArgs(source, listeningAudioState));
        }

        public void InvokeHeardSomethingEvent(Source source, string phrase)
        {
            var handler = HeardSomethingEvent;
            if (handler != null) handler(this, new HeardSomethingEventArgs(source, phrase));
        }

        public void InvokeStartTalkingEvent(Guid talkId, string bitchName, string phrase)
        {
            var handler = StartTalkingEvent;
            if (handler != null) handler(this, new StartTalkingEventArgs(talkId, bitchName, phrase));
        }

        public void InvokeDoneTalkingEvent(Guid talkId)
        {
            var handler = DoneTalkingEvent;
            if (handler != null) handler(this, new DoneTalkingEventArgs(talkId));
        }

        public void InvokeStartListeningEvent(Guid listenId)
        {
            var handler = StartListeningEvent;
            if (handler != null) handler(this, new StartListeningEventArgs(listenId));
        }

        public void InvokeDoneListeningEvent(Guid listenId)
        {
            var handler = DoneListeningEvent;
            if (handler != null) handler(this, new DoneListeningEventArgs(listenId));
        }

        public void InvokeStartWorkingEvent(Guid workId, string name)
        {
            var handler = StartWorkingEvent;
            if (handler != null) handler(this, new StartWorkingEventArgs(workId, name));
        }

        public void InvokeDoneWorkingEvent(Guid workId)
        {
            var handler = DoneWorkingEvent;
            if (handler != null) handler(this, new DoneWorkingEventArgs(workId));
        }
    }
}