using System;
using System.Collections.Concurrent;
using System.Threading;
using common.core;
using Microsoft.Xna.Framework;

namespace common.command
{
    public class TextInputManager
    {
        private Thread _readThread;
        private bool _enabled;
        private IGame _game;
        private ConcurrentQueue<CommandRunner> _commandRunners;

        public TextInputManager(IGame game)
        {
            _game = game;
            _enabled = false;
            _commandRunners = new ConcurrentQueue<CommandRunner>();
            _readThread = new Thread(ListenForCommand);
        }

        public void Start()
        {
            _enabled = true;
            _readThread.Start();
        }

        public void Update()
        {
            while (!_commandRunners.IsEmpty)
            {
                if (_commandRunners.TryDequeue(out var runner))
                {
                    runner.Run();
                }
            }
        }
        private void ListenForCommand()
        {
            while (_enabled)
            {
                var input = Console.ReadLine();
                if (input == null || !input.StartsWith("/")) 
                    continue;
                input = input.Remove(0,1);
                var runner = new CommandRunner(input.Split(" ", StringSplitOptions.TrimEntries), new CommandContext(_game));
                _commandRunners.Enqueue(runner);

            }
        }
        public void Stop()
        {
            _enabled = false;
            _readThread.Join();
        }
    }
}