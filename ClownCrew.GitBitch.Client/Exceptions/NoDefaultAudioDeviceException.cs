using System;

namespace ClownCrew.GitBitch.Client.Exceptions
{
    internal class NoDefaultAudioDeviceException : InvalidOperationException
    {
        public NoDefaultAudioDeviceException(Exception innerException)
            :base("Unable to find default device for audio input.", innerException)
        {
        }
    }
}