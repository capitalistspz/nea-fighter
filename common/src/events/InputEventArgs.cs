using System;
using common.data;
using common.physics;
using Microsoft.Xna.Framework;

namespace common.events
{
    public class InputEventArgs : EventArgs
    {
        public ushort LocalPlayerId = 0;
        public BitField Actions = new (0);
        public Vector2I MovementDirection = Vector2I.Zero;
        public Vector2 AimDirection = Vector2.Zero;
    }
}