using System;
using common.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace common.entities
{
    // Template for all entities
    public abstract class BaseEntity : ICollisionActor
    {
        public readonly World CurrentWorld;
        // For unique identification of entities
        public Guid Id { get; set; }
        public abstract IShapeF Bounds { get; }
        public bool Visible { get; set; }
        public Point2 Position
        {
            get => Bounds.Position;
            set => Bounds.Position = value;
        }

        public Vector2 Velocity { get; set; }
        public BaseEntity(World world, Guid id, Vector2 position)
        {
            Id = id;
            CurrentWorld = world;
        }
        public abstract void OnCollision(CollisionEventArgs collisionInfo);
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SpriteBatch spriteBatch){}
    }

    
}