using ClownCrew.GitBitch.Client.Agents;

namespace ClownCrew.GitBitch.Client
{
    public partial class App
    {
        public App()
        {
            var talkAgent = new TalkAgent();
            talkAgent.Say("Hi! I'm git bitch alfa. You didn't think that my first words would be, Dada, or something stupid like that, did you?");
        }
    }
}
