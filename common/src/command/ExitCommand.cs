namespace common.command
{
    public class ExitCommand
    {
        public static void Register()
        {
            var root = new CommandNode("exit", CommandInputType.Literal)
                .Run(cmd => cmd.GetGame().Exit());
            Commands.AddCommand(root, "Closes the application");
        }
    }
}