using System;
using common.core;
using common.entities;
using common.events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace client.entities
{
    // Local version of player entity for inputs and drawing
    public class ClientPlayerEntity : PlayerEntity
    {
        public ClientPlayerEntity(World world, Guid id, Vector2 position, ushort localPlayerId) : base(world, id, position)
        {
            LocalPlayerID = localPlayerId;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, Color.White);
        }
        
    }
}