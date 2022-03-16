using System;
using MonoGame.Extended.Collections;

namespace common.command
{
    public class CommandNode
    {
        private string _name;
        private CommandInputType _commandInputType;
        private Action<CommandContext> _action;

        private KeyedCollection<string, CommandNode> _children;

        public CommandNode(string name, CommandInputType type)
        {
            _children = new KeyedCollection<string, CommandNode>(e => e._name);
            _name = name;
            _commandInputType = type;
        }
        public CommandNode Next(CommandNode nextNode)
        {
            _children.Add(nextNode);
            return this;
        }

        public CommandNode Run(Action<CommandContext> cmd)
        {
            _action = cmd;
            return this;
        }

        public Action<CommandContext> GetAction()
        {
            return _action;
        }

        public CommandNode? GetChild(string childName)
        {
            return _children.TryGetValue(childName, out var node) ? node : null;
        }

        public bool IsLiteral()
        {
            return _commandInputType == CommandInputType.Literal;
        }

        public string GetName()
        {
            return _name;
        }
    }
}