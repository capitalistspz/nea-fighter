using client.entities;
using Microsoft.Xna.Framework;

namespace client.input
{
    // Base for all input managers
    public interface IInputManager<T>
    {
        public Vector2 AimDirection { get; }
        // The thing controlled controlled by the input
        public T Owner { get; set; }
        public void Update(GameTime gameTime);
    }
}