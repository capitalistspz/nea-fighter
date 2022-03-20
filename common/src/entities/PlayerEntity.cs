using System;
using common.core;
using common.events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Serilog;

namespace common.entities
{
    public class PlayerEntity : BaseEntity
    {
        // Time left until player does not count as being on the ground
        public int OnGround { get; set; }
        public float Gravity { get; set; }
        public ushort LocalPlayerID { get; protected set; }
        public override Vector2 MaxSpeed { get; protected set; }
        public override IShapeF Bounds { get; }

        // To prevent attack spam
        private float _attackCooldown;
        private float _timeSinceLastAttack;
        
        public string Name = String.Empty;
        public static Texture2D PlayerTexture;
        public PlayerEntity(World world, Guid id, Vector2 position) : base(world, id, position)
        {
            Visible = true;
            Bounds = new RectangleF(position, new Size2(128, 128));
            MaxSpeed = new Vector2(10, 20);
            Gravity = 0.3f;
            _attackCooldown = 100;
        }

        public override void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is PlayerEntity)
            {
                Bounds.Position -= collisionInfo.PenetrationVector;
                collisionInfo.Other.Bounds.Position += collisionInfo.PenetrationVector;
                
                //var velocity = GetVelocity() - collisionInfo.PenetrationVector * 0.4f;
                //SetVelocity(velocity);
            }
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / 10);
            var velX = Math.Abs(Velocity.X) > 0.1f ? Velocity.X * 0.9f : 0;
            var velY = Velocity.Y + 0.5f * Gravity * deltaTime;
            Velocity = Vector2.Clamp(new Vector2(velX, velY), -MaxSpeed, MaxSpeed);
            Position += Velocity * deltaTime;
            OnGround -= 1;
            _timeSinceLastAttack += deltaTime;
            base.Update(gameTime);
        }

        public void ApplyInput(InputEventArgs args)
        {
            var actions = args.Actions;
            if (actions[0])
                Jump();
            if (_timeSinceLastAttack > _attackCooldown)
            {
                if (actions[1])
                    Lunge(args.AimDirection);
                if (actions[2])
                    Shoot(args.AimDirection);
                _timeSinceLastAttack = 0;

            }

            Velocity += args.MovementDirection * 2;
            GameEvents.EnqueueInputEvent(args);
        }

        private void Shoot(Vector2 argsAimDirection)
        {
            //TODO:Add projectile firing;
        }

        private void Lunge(Vector2 direction)
        {
            Velocity += direction * 3;
        }

        private void Jump()
        {
            if (OnGround > 0)
            {
                Velocity = new Vector2(Velocity.X, -5f);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(PlayerTexture, Position, Color.White);
        }
    }
}