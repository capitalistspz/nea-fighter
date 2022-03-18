using System.Collections.Generic;

namespace common.command
{
    public static class Commands
    {
        private static CommandNode _commands;
        private static Dictionary<string, string> _helpMessages;

        static Commands()
        {
            _commands = new CommandNode("base", CommandInputType.Literal);
            _helpMessages = new Dictionary<string, string>();
        }
        public static void AddCommand(CommandNode command)
        {
            _commands.Next(command);
        }
        
        public static void AddCommand(CommandNode command, string helpMessage)
        {
            _helpMessages.Add(command.GetName(), helpMessage);
            _commands.Next(command);
        }

        public static CommandNode? GetChild(string name)
        {
            return _commands.GetChild(name);
        }

        public static CommandNode GetRoot()
        {
            return _commands;
        }

        public static string GetHelpMessage(string command)
        {
            return _helpMessages[command];
        }
    }
}