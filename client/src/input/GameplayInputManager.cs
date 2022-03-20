using System.Collections.Generic;
using client.entities;
using common.events;
using Microsoft.Xna.Framework;

namespace client.input
{
    // Handles inputs that occur during gameplay
    public abstract class GameplayInputManager<TInputElement> : IInputManager<ClientPlayerEntity>
    {
        private long _lastUpdateTime;
        private long _minTimeDelay;
        
        private Dictionary<InputAction, TInputElement> _inputMapping;
        
        public ClientPlayerEntity Owner { get; set; }
        public abstract Vector2 AimDirection { get; }
        
        public GameplayInputManager(ClientPlayerEntity owner)
        {
            _inputMapping = new Dictionary<InputAction, TInputElement>();
            _lastUpdateTime = 0;
            _minTimeDelay = 200000;
            Owner = owner;
        }
        public void AssignInput(InputAction action, TInputElement inputElement)
        {
            _inputMapping[action] = inputElement;
        }

        public void AssignInputs(IEnumerable<KeyValuePair<InputAction, TInputElement>> mapping)
        {
            _inputMapping = new Dictionary<InputAction,TInputElement> (mapping);

        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Ticks - _lastUpdateTime < _minTimeDelay)
                return;

            var args = new InputEventArgs {LocalPlayerId = Owner.LocalPlayerID};
            
            var updated = false;
            if (IsPressed(_inputMapping[InputAction.Jump]))
            {
                args.Actions[0] = true;
                updated = true;
            }

            if (IsPressed(_inputMapping[InputAction.Attack1]))
            {
                args.Actions[1] = true;
                updated = true;
            }

            if (IsPressed(_inputMapping[InputAction.Attack2]))
            {
                args.Actions[2] = true;
                updated = true;
            }

            if (IsPressed(_inputMapping[InputAction.Block]))
            {
                args.Actions[3] = true;
                updated = true;
            }
            if (IsPressed(_inputMapping[InputAction.Left]))
            {
                args.MovementDirection.X = -1;
                updated = true;
            }
            else if (IsPressed(_inputMapping[InputAction.Right]))
            {
                args.MovementDirection.X = 1;
                updated = true;
            }
            if (IsPressed(_inputMapping[InputAction.Down]))
            {
                args.MovementDirection.Y = 1;
                updated = true;
            }
            else if (IsPressed(_inputMapping[InputAction.Up]))
            {
                args.MovementDirection.Y = -1;
                updated = true;
            }
            args.AimDirection = AimDirection;
            if (updated)
            {
                Owner.ApplyInput(args);
                _lastUpdateTime = gameTime.TotalGameTime.Ticks;
            }
        }
        public abstract bool IsPressed(TInputElement key);

    }
}