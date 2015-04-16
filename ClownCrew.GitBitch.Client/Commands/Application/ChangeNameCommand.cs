using System.Collections.Generic;
using System.IO;
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
            if (File.Exists("Names.txt")) names.AddRange(File.ReadAllLines("Names.txt"));

            var response = new Answer<bool>(false);
            var bitchName = new Answer<string>(names.First());
            while (!response.Response)
            {
                //TODO: Remove this line after that the text-input question is enabled. Before we have that this will result in an infinite loop.
                if (_settingAgent.GetSetting("Muted", false))
                {
                    await _talkAgent.SayAsync("You are muted to me, so I will not hear what you are saying. Microphone mute is enabled automatically when there is a technical problem. You can also do this manually.");
                    return;
                }
                //TODO: ^^ This crappy part above, remove it when the AskAsnyc method can handle manual user input that does not come from the microphone ^^

                bitchName = await _talkAgent.AskAsync("What do you want my name to be?", names.Select(x => new QuestionAnswerAlternative<string> { Phrases = new List<string> { x }, Response = x }).ToList(), 5000);
                response = await _talkAgent.AskAsync(string.Format("So you want my name to be {0}?", bitchName.Response), new List<QuestionAnswerAlternative<bool>> { new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "Yes" }, Response = true, IsDefault = hasSetting }, new QuestionAnswerAlternative<bool> { Phrases = new List<string> { "No" }, Response = false, IsDefault = !hasSetting } });
            }

            _settingAgent.SetSetting(Constants.BitchName, bitchName.Response);
            App.RegisterCommands();
            await _talkAgent.SayAsync(string.Format("Allright, {0} it is.", bitchName.Response));
        }

        public static List<string> GetDefaultNames(string defaultName)
        {
            return new List<string> { defaultName, "Git", "Bitch", "Ivona", "Astra", "Zira", "Leeloominai", "Leeloo", "Master", "Commander", "Mister", "Miss", "Mistress" };
        }
    }
}