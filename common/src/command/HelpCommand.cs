using System;

namespace common.command
{
    public class HelpCommand
    {
        public static void Register()
        {
            var command = new CommandNode("help", CommandInputType.Literal)
                .Next(new CommandNode("command", CommandInputType.Argument)
                    .Run(cmd => 
                    {
                        var commandName = cmd.GetArg("command");
                        var helpMessage = Commands.GetHelpMessage(commandName);
                        Console.WriteLine($"Help for {commandName}: {helpMessage}");
                    
                    }));
                
            Commands.AddCommand(command, "Get information on what a command does and its syntax.\n" +
                                         "/help command:<command>\n" +
                                         "Example: /help command:help");
        }
    }
}