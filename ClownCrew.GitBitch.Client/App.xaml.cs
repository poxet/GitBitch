using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
            Dispatcher.UnhandledException += UnhandledException;
            CompositeRoot.Instance.Notifyer.Show();
        }

        private void UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CompositeRoot.Instance.TalkAgent.SayAsync("Oups, now we have problems! " + e.Exception.Message);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CloseCommand.CloseDownEvent += CloseDownEvent;

            await SetBitchNameAsync();
            RegisterCommands();
            await Greeting();
            await CompositeRoot.Instance.TalkAgent.SayAsync("What can I help you with?");
        }

        private static async Task SetBitchNameAsync()
        {
            if (CompositeRoot.Instance.SettingAgent.HasSetting(Constants.BitchName)) return;
            await CompositeRoot.Instance.TalkAgent.SayAsync("Hi! This is git bitch alfa.");
            await new ChangeNameCommand(CompositeRoot.Instance.SettingAgent, CompositeRoot.Instance.TalkAgent).ExecuteAsync(string.Empty, string.Empty);
            CompositeRoot.Instance.SettingAgent.UseAutoStart(true);
        }

        private static async Task Greeting()
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
            if (File.Exists("Names.txt")) names.AddRange(File.ReadAllLines("Names.txt"));
            var alternatives = names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList();
            var response = await CompositeRoot.Instance.QuestionAgent.AskAsync("What is your name?", alternatives, 12000);
            if (response.Response == Constants.DefaultBitchName) await CompositeRoot.Instance.TalkAgent.SayAsync("Have it your way, I will just call you " + response.Response + " then.");
            else await CompositeRoot.Instance.TalkAgent.SayAsync("O my god, you can't be serious. How am I supposed to work with someone named " + response.Response + ".");

            CompositeRoot.Instance.SettingAgent.SetSetting("YourName", response.Response);
        }

        private void CloseDownEvent(object sender, EventArgs e)
        {
            //TODO: Tell the window that it is allowed to close down. (Instead of be hidden)
            Dispatcher.Invoke(Shutdown);
        }

        public static void RegisterCommands()
        {
            CompositeRoot.Instance.CommandAgent.ClearCommands();
            
            //TODO: Look for commands by convension. (Register all classes that inherits from GitBitchCommand)
            
            CompositeRoot.Instance.CommandAgent.RegisterCommands(new ApplicationCommands());
            CompositeRoot.Instance.CommandAgent.RegisterCommands(new WindowsCommands());
            CompositeRoot.Instance.CommandAgent.RegisterCommands(new GitCommands());
            
            //TODO: Also look for dll's in the main folder to load stuff from there
        }
    }
}