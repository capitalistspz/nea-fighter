using System;
using common.entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Serilog;

namespace common.physics
{
    // A simple rectangular object that can be collided with
    // Used to implement tiled map collisions
    public class BasicCollisionObject : ICollisionActor
    {
        public IShapeF Bounds { get; }

        public BasicCollisionObject(RectangleF collisionBounds)
        {
            Bounds = collisionBounds;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is BaseEntity entity)
            {
                entity.Position += collisionInfo.PenetrationVector;
                if (entity is PlayerEntity player)
                {
                    if (collisionInfo.PenetrationVector.X > collisionInfo.PenetrationVector.Y)
                    {
                        player.OnGround = 20;
                        if (player.Velocity.Y > 0)
                            player.Velocity.SetY(0);
                    }
                }
            }
        }
    }
}