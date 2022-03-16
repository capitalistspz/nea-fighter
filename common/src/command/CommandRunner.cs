using System;
using System.Collections.Generic;

namespace common.command
{
    public class CommandRunner
    {
        private readonly string[] _parts;
        private readonly CommandContext _ctx;

        public CommandRunner(string[] commandParts, CommandContext ctx)
        {
            _parts = commandParts;
            _ctx = ctx;
        }
        public void Run()
        {
            var temp = new Queue<string>(_parts);
            var iterator = Commands.GetRoot();
            try
            {
                while (temp.Count > 0)
                {
                    var txt = temp.Dequeue();
                    var split = txt.Split(":");
                    iterator = iterator.GetChild(split[0]);
                    if (iterator == null)
                        throw new BadCommandException(txt);
                    if (!iterator.IsLiteral())
                    {
                        _ctx.SetArg(iterator.GetName(), split[1]);
                    }
                }

                var action = iterator.GetAction();
                action(_ctx);
            }
            catch (BadCommandException e)
            {

                var prevForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Bad command: {e.Message}");
                Console.ForegroundColor = prevForegroundColor;

            }
            catch (NullReferenceException e)
            {
                var prevForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Command does not exist");
                Console.ForegroundColor = prevForegroundColor;
            }
            catch (Exception ignored)
            {
                // ignored
            }
            
            
        }
    }
    public class BadCommandException : Exception
    {
        public BadCommandException(string msg) : base(msg)
        {
            
        }
    }
}