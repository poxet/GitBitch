using System.Windows;

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
        }
    }
}