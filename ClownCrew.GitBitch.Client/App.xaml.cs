using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClownCrew.GitBitch.Client.Commands.Application;
using ClownCrew.GitBitch.Client.Commands.Git;
using ClownCrew.GitBitch.Client.Commands.Windows;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client
{    
    public partial class App
    {
        public App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            CompositeRoot.Instance.Notifyer.Show();
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            CompositeRoot.Instance.TalkAgent.SayAsync("Oups, now we have problems! " + e.Exception.Message);
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await SetBitchNameAsync();
            await RegisterCommandsAsync();
            await Greeting();
            await CompositeRoot.Instance.TalkAgent.SayAsync("What can I help you with?");

            CloseCommand.CloseDownEvent += CloseDownEvent;
        }

        public static async Task SetBitchNameAsync()
        {
            if (CompositeRoot.Instance.SettingAgent.HasSetting(Constants.BitchName)) return;
            await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! This is git bitch alfa.");
            await new ChangeNameCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.TalkAgent).ExecuteAsync(string.Empty, string.Empty);
            CompositeRoot.Instance.SettingAgent.UseAutoStart(true);
        }

        public static async Task Greeting()
        {
            if (CompositeRoot.Instance.SettingAgent.HasSetting("YourName"))
            {
                var yourname = CompositeRoot.Instance.SettingAgent.GetSetting("YourName", Constants.BitchName);
                await CompositeRoot.Instance.TalkAgent.SayAsync("Wellcome back " + yourname + ".");
                return;
            }

            var bitchName = CompositeRoot.Instance.SettingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);
            await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! this is git bitch alfa. My name is " + bitchName + ".");

            await AskForOperstorsName();
        }

        private static async Task AskForOperstorsName()
        {
            var names = ChangeNameCommand.GetDefaultNames(Constants.DefaultOperatorName);
            if (System.IO.File.Exists("Names.txt")) names.AddRange(System.IO.File.ReadAllLines("Names.txt"));
            var alternatives = names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList();
            var response = await CompositeRoot.Instance.QuestionAgent.AskAsync("What is your name?", alternatives, 12000);
            if (response.Response == Constants.DefaultBitchName) await CompositeRoot.Instance.TalkAgent.SayAsync("Have it your way, I will just call you " + response.Response + " then.");
            else await CompositeRoot.Instance.TalkAgent.SayAsync("O my god, you can't be serious. How am I supposed to work with someone named " + response.Response + ".");

            CompositeRoot.Instance.SettingAgent.SetSetting("YourName", response.Response);
        }

        private void CloseDownEvent(object sender, EventArgs e)
        {
            Dispatcher.Invoke(Shutdown);
        }

        public static async Task RegisterCommandsAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                CompositeRoot.Instance.CommandAgent.ClearCommands();
                CompositeRoot.Instance.CommandAgent.RegisterCommands(new ApplicationCommands());
                CompositeRoot.Instance.CommandAgent.RegisterCommands(new WindowsCommands());
                CompositeRoot.Instance.CommandAgent.RegisterCommands(new GitCommands());
            });
        }
    }
}