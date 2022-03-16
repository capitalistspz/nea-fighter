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
        private Point2 _serverSidePosition;
        private Point2 _oldPosition;
        private Vector2 _serverSideVelocity;
        public abstract Vector2 MaxSpeed { get; protected set; }

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
        // Used by the client to interpolate the delayed movement received from the server 
        public void AssignServerMotion(Point2 position, Vector2 velocity)
        {
            _ticksSinceLerpStart = 0;
            _serverSideVelocity = velocity;
            _serverSidePosition = position;
            _oldPosition = Position;
        }
        public abstract void OnCollision(CollisionEventArgs collisionInfo);

        public virtual void Update(GameTime gameTime)
        {
            if (_ticksSinceLerpStart >= 0 && _ticksSinceLerpStart < _maxLerpTicks)
            {
                var interpolationDelta = (float) _ticksSinceLerpStart / _maxLerpTicks;
                var intX = MathHelper.Lerp(_oldPosition.X, _serverSidePosition.X, interpolationDelta);
                var intY = MathHelper.Lerp(_oldPosition.Y, _serverSidePosition.Y, interpolationDelta);
                Position = new Point2(intX, intY);
                
                var intVx = MathHelper.Lerp(Velocity.X, _serverSideVelocity.X, interpolationDelta);
                var intVy = MathHelper.Lerp(Velocity.Y, _serverSideVelocity.Y, interpolationDelta);
                Velocity = new Vector2(intVx, intVy);
                
                _ticksSinceLerpStart += gameTime.ElapsedGameTime.Ticks;
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch){}
    }

    
}