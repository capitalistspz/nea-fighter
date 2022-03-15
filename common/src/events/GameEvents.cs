using System;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework;

namespace common.events
{
    public static class GameEvents
    {
        public static EventHandler<InputEventArgs> InputEvent;

        private static ConcurrentQueue<InputEventArgs> _inputEvents;
        
        public static void EnqueueInputEvent(InputEventArgs args)
        {
            _inputEvents.Enqueue(args);
        }

        public static void TriggerInputEvents()
        {
            while (!_inputEvents.IsEmpty)
            {
                if (_inputEvents.TryDequeue(out var args))
                    InputEvent?.Invoke(null, args);
            }
        }

    }
}