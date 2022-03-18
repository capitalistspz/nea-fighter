using System.Collections.Generic;
using common.core;
using Microsoft.Xna.Framework;

namespace common.command
{
    public class CommandContext
    {
        private readonly Dictionary<string, string> _args;
        private IGame _game;
        public CommandContext(IGame game)
        {
            _game = game;
            _args = new Dictionary<string, string>();
        }

        public string GetArg(string argName)
        {
            return _args[argName];
        }

        public void SetArg(string argName, string value)
        {
            _args[argName] = value;
        }

        public IGame GetGame()
        {
            return _game;
        }
    }
}