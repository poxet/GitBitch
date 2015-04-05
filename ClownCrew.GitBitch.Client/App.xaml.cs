using System;
using System.Windows;
using ClownCrew.GitBitch.Client.Commands.Application;

namespace ClownCrew.GitBitch.Client
{
    public partial class App
    {
        public App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            CompositeRoot.Instance.TalkAgent.SayAsync("Oups, now we have problems! " + e.Exception.Message);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! I'm git bitch alfa. You didn't think that my first words would be, Dada, or something stupid like that, did you?");

            CloseCommand.CloseDownEvent += CloseDownEvent;
        }

        private void CloseDownEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(Shutdown);
        }
    }
}