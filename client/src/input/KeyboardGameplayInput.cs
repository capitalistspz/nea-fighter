using System.Collections.Generic;
using client.entities;
using client.input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace client.input
{
    public class KeyboardGameplayInput : GameplayInput<Keys>
    {
        public override bool IsPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public override Vector2 AimDirection
        {
            get
            {
                var ownerBounds = (RectangleF) Owner.Bounds;
                var posDiff = Mouse.GetState().Position - ownerBounds.Center;
                posDiff.Normalize();
                return posDiff;
            }
        }

        public KeyboardGameplayInput(ClientPlayerEntity owner) : base(owner)
        {
            AssignInputs(DefaultMapping);
        }
        public static Dictionary<InputAction, Keys> DefaultMapping => new(8)
        {
            [InputAction.Jump] = Keys.Space,
            [InputAction.Attack1] = Keys.F,
            [InputAction.Attack2] = Keys.G,
            [InputAction.Block] = Keys.Z,
            [InputAction.Left] = Keys.A,
            [InputAction.Right] = Keys.D,
            [InputAction.Up] = Keys.W,
            [InputAction.Down] = Keys.S
                
        };
        
        public static Dictionary<InputAction, Keys> BlankMapping => new(8)
        {
            [InputAction.Jump] = Keys.None,
            [InputAction.Attack1] = Keys.None,
            [InputAction.Attack2] = Keys.None,
            [InputAction.Block] = Keys.None,
            [InputAction.Left] = Keys.None,
            [InputAction.Right] = Keys.None,
            [InputAction.Up] = Keys.None,
            [InputAction.Down] = Keys.None,
        };
    }
    
}