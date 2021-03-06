using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ClownCrew.GitBitch.Client.Agents;
using ClownCrew.GitBitch.Client.Commands.Application;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model.EventArgs;
using ClownCrew.GitBitch.Client.Views;
using Application = System.Windows.Application;

namespace ClownCrew.GitBitch.Client
{
    public class Notifyer : INotifyer, IDisposable
    {
        private readonly NotifyIcon _notifyIcon = new NotifyIcon();
        private SettingsWindow _settingsWindow;

        public Notifyer(IEventHub eventHub, ICommandAgent commandAgent)
        {
            var asmName = Assembly.GetExecutingAssembly().GetName().Name;
            var iconStream = GetType().Assembly.GetManifestResourceStream(asmName + ".Resources.GitBitch.ico");
            if (iconStream != null) _notifyIcon.Icon = new Icon(iconStream);

            eventHub.StartTalkingEvent += StartTalkingEvent;
            commandAgent.CommandRegisteredEvent += CommandRegisteredEvent;
            _notifyIcon.ContextMenu = new ContextMenu(new[] { new MenuItem("Open", OpenEventHandler), new MenuItem("Settings", SettingsEventHandler), new MenuItem("Exit", ExitEventHandler) });
        }

        private void CommandRegisteredEvent(object sender, CommandRegisteredEventArgs e)
        {
            var commandsNode = _notifyIcon.ContextMenu.MenuItems["Commands"];
            if (commandsNode == null)
            {
                commandsNode = new MenuItem { Text = @"Commands", Name = "Commands" };
                _notifyIcon.ContextMenu.MenuItems.Add(commandsNode);
            }

            var section = commandsNode.MenuItems[e.SectionName];
            if (section == null)
            {
                section = new MenuItem { Text = e.SectionName, Name = e.SectionName };
                commandsNode.MenuItems.Add(section);
            }

            section.MenuItems.Add(new MenuItem(e.Command.Name, (s, e1) => e.Command.ExecuteAsync(null, null)) { Name = e.Command.Name });
        }

        private void OpenEventHandler(object sender, EventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).Show();
            ((MainWindow)Application.Current.MainWindow).Focus();
        }

        private void SettingsEventHandler(object sender, EventArgs e)
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new SettingsWindow();
                _settingsWindow.Show();
                _settingsWindow.Closed += SettingsWindowClosed;
            }
            else
            {
                _settingsWindow.Show();
                _settingsWindow.Focus();
            }
        }

        private void SettingsWindowClosed(object sender, EventArgs e)
        {
            _settingsWindow = null;
        }

        private void ExitEventHandler(object sender, EventArgs e)
        {
            CloseCommand.InvokeCloseDownEvent();
        }

        private void StartTalkingEvent(object sender, StartTalkingEventArgs e)
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