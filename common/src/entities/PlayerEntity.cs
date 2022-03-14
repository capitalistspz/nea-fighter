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
        // Jump only when on the ground and if not jump
        public int OnGround { get; set; }
        public float Gravity { get; set; }

        // To prevent attack spam
        private float _attackCooldown;
        private float _timeSinceLastAttack;
        
        public string Name;
        public static Texture2D PlayerTexture;
        public PlayerEntity(World world, Guid id, Vector2 position) : base(world, id, position)
        {
            Visible = true;
            Bounds = new RectangleF(position, new Size2(128, 128));
            Gravity = 1f;
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
            Velocity += new Vector2(0, 0.5f * Gravity * deltaTime);
            Position += Velocity * deltaTime;
            Velocity *= 0.8f;
            
            OnGround -= 1;
            _timeSinceLastAttack += deltaTime;
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
            Log.Debug("Attempted to jump, ground time {OnGround}", OnGround);
            if (OnGround > 0)
            {
                Velocity += new Vector2(0, -6f);
            }
        }

        public override IShapeF Bounds { get; }
    }
}