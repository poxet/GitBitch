using System;
using System.Reflection;
using System.Windows.Forms;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Views;

namespace ClownCrew.GitBitch.Client
{
    public class Notifyer : INotifyer, IDisposable
    {
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();

        public Notifyer(IEventHub eventHub)
        {
            var asmName = Assembly.GetExecutingAssembly().GetName().Name;
            var iconStream = GetType().Assembly.GetManifestResourceStream(asmName + ".Resources.GitBitch.ico");
            if (iconStream != null) _notifyIcon.Icon = new System.Drawing.Icon(iconStream);

            eventHub.StartTalkingEvent += StartTalkingEvent;
            _notifyIcon.BalloonTipClicked += BalloonTipClicked;
        }

        private void BalloonTipClicked(object sender, EventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).Show();
        }

        private void StartTalkingEvent(object sender, Model.EventArgs.StartTalkingEventArgs e)
        {
            _notifyIcon.ShowBalloonTip(5000, e.Name, e.Phrase, ToolTipIcon.Info);            
        }

        public void Dispose()
        {            
            _notifyIcon.Dispose();
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
        }        
    }
}