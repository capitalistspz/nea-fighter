using System;
using common.core;
using common.entities;
using Microsoft.Xna.Framework;

namespace server.entities
{
    public class ServerPlayerEntity : PlayerEntity
    {
        public ServerPlayerEntity(World world, Guid id, Vector2 position, ushort localPlayerId) : base(world, id, position)
        {
            LocalPlayerID = localPlayerId;
        }
    }
}