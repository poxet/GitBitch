using System;
using System.Drawing;
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
            _notifyIcon.Icon = new Icon(@"Resources/GitBitch.ico");
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