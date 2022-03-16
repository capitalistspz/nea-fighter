using System;
using common.core;
using common.entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace client.entities
{
    // Local version of player entity for inputs and drawing
    public class ClientPlayerEntity : PlayerEntity
    {
        public OrthographicCamera Camera { get; set; }
        public ClientPlayerEntity(World world, Guid id, Vector2 position, ushort localPlayerId) : base(world, id, position)
        {
            LocalPlayerID = localPlayerId;
        }
    }
}