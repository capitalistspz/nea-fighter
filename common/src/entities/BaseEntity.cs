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
        private long _ticksSinceLerpStart;
        private long _maxLerpTicks;
        public Point2 serverSidePosition;
        private Point2 _oldPosition;

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
            _ticksSinceLerpStart = -1;
            _maxLerpTicks = 100;
        }
        public void AssignServerMotion(Point2 position, Vector2 velocity)
        {
            _ticksSinceLerpStart = 0;
            serverSidePosition = position;
            _oldPosition = Position;
            Velocity = (velocity + Velocity) / 2;

        }
        public abstract void OnCollision(CollisionEventArgs collisionInfo);

        public virtual void Update(GameTime gameTime)
        {
            if (_ticksSinceLerpStart >= 0 && _ticksSinceLerpStart < _maxLerpTicks)
            {
                var intX = MathHelper.Lerp(_oldPosition.X, serverSidePosition.X, (float)_ticksSinceLerpStart / _maxLerpTicks);
                var intY =MathHelper.Lerp(_oldPosition.Y, serverSidePosition.Y, (float)_ticksSinceLerpStart / _maxLerpTicks);
                Position = new Point2(intX, intY);
                _ticksSinceLerpStart += gameTime.ElapsedGameTime.Ticks;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch){}
    }

    
}