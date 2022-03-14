using System;
using System.Collections.Generic;
using client.entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace client.input
{
    public class ControllerGameplayInput : GameplayInput<Buttons>
    {
        // MonoGame accesses controllers by index
        public readonly int ControllerId;
        public override Vector2 AimDirection => GamePad.GetState(ControllerId).ThumbSticks.Right;
       
        public override bool IsPressed(Buttons button)
        {
            return GamePad.GetState(ControllerId).IsButtonDown(button);
        }
        
        public ControllerGameplayInput(ClientPlayerEntity owner, int controllerId) : base(owner)
        {
            AssignInputs(DefaultMapping);
            if (controllerId >= 0)
                ControllerId = controllerId;
            else
                throw new ArgumentOutOfRangeException(nameof(controllerId));
        }

        public Dictionary<InputAction, Buttons> DefaultMapping => new()
        {

            [InputAction.Jump] = Buttons.A,
            [InputAction.Attack1] = Buttons.B,
            [InputAction.Attack2] = Buttons.Y,
            [InputAction.Block] = Buttons.X,
            [InputAction.Left] = Buttons.DPadLeft,
            [InputAction.Right] = Buttons.DPadRight,
            [InputAction.Up] = Buttons.DPadUp,
            [InputAction.Down] = Buttons.DPadDown
        };
        
    }
}