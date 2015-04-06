using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class ChangeNameCommand : GitBitchCommand
    {
        public ChangeNameCommand(ISettingAgent settingAgent)
            : base(settingAgent, "Change name", new[] { "change name" })
        {
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            var hasSetting = CompositeRoot.Instance.SettingAgent.HasSetting(Constants.BitchName);
            var currentName = CompositeRoot.Instance.SettingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);

            var names = new List<string> { currentName, Constants.DefaultBitchName, "Ivona", "Astra", "Zira", "Leeloominai", "Leeloo" };
            if (System.IO.File.Exists("Names.txt")) names.AddRange(System.IO.File.ReadAllLines("Names.txt"));

            var response = new Answer<bool>(false);
            var bitchName = new Answer<string>(names.First());
            while (!response.Response)
            {
                bitchName = await CompositeRoot.Instance.TalkAgent.AskAsync("What do you want my name to be?", names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList(), 5000);
                response = await CompositeRoot.Instance.TalkAgent.AskAsync(string.Format("So you want my name to be {0}?", bitchName.Response), new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true, IsDefault = hasSetting }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = !hasSetting } });
            }
            
            CompositeRoot.Instance.SettingAgent.SetSetting(Constants.BitchName, bitchName.Response);
            await App.RegisterCommandsAsync();
            await CompositeRoot.Instance.TalkAgent.SayAsync(string.Format("Allright, {0} it is.", bitchName.Response));
        }
    }
}