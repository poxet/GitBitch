using System.Collections.Generic;

namespace ClownCrew.GitBitch.Client.Commands.Application
{
    public static class StringListExtensions
    {
        public static string ToAndList(this IEnumerable<string> values)
        {
            var commands = string.Join(", ", values);
            var pos = commands.LastIndexOf(", ", System.StringComparison.Ordinal);
            if (pos == -1)
            {
                return commands;
            }

            commands = commands.Substring(0, pos) + " and " + commands.Substring(pos + 2);
            return commands;
        }
    }
}