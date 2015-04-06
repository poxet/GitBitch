using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClownCrew.GitBitch.Client.Interfaces;
using ClownCrew.GitBitch.Client.Model;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public class ChangeNameCommand : GitBitchCommand
    {
        private readonly ITalkAgent _talkAgent;

        public ChangeNameCommand(ISettingAgent settingAgent, ITalkAgent talkAgent)
            : base(settingAgent, "Change name", new[] { "change name" })
        {
            _talkAgent = talkAgent;
        }

        public override async Task ExecuteAsync(string key, string phrase)
        {
            var hasSetting = _settingAgent.HasSetting(Constants.BitchName);
            var currentName = _settingAgent.GetSetting(Constants.BitchName, Constants.DefaultBitchName);

            var names = GetDefaultNames(Constants.DefaultBitchName);
            if (System.IO.File.Exists("Names.txt")) names.AddRange(System.IO.File.ReadAllLines("Names.txt"));

            var response = new Answer<bool>(false);
            var bitchName = new Answer<string>(names.First());
            while (!response.Response)
            {
                bitchName = await _talkAgent.AskAsync("What do you want my name to be?", names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList(), 5000);
                response = await _talkAgent.AskAsync(string.Format("So you want my name to be {0}?", bitchName.Response), new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true, IsDefault = hasSetting }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = !hasSetting } });
            }

            _settingAgent.SetSetting(Constants.BitchName, bitchName.Response);
            await App.RegisterCommandsAsync();
            await _talkAgent.SayAsync(string.Format("Allright, {0} it is.", bitchName.Response));
        }

        public static List<string> GetDefaultNames(string defaultName)
        {
            return new List<string> { defaultName, "Git", "Bitch", "Ivona", "Astra", "Zira", "Leeloominai", "Leeloo", "Master", "Commander", "Mister", "Miss", "Mistress" };
        }
    }
}