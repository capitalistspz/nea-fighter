using common.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace client.core
{
    // Same as World but can render visuals
    public class ClientWorld : World
    {
        // So that the map can be rendered
        private TiledMapRenderer _tiledMapRenderer;
        public ClientWorld(TiledMap map, GraphicsDevice graphicsDevice) : base(map)
        {
            _tiledMapRenderer = new TiledMapRenderer(graphicsDevice, map);
        }
        protected new void Update(GameTime gameTime)
        {
            _tiledMapRenderer.Update(gameTime);
            base.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Matrix mapTransform)
        {
            _tiledMapRenderer.Draw(mapTransform);
            foreach (var visibleEntity in FilterEntities(e => e.Visible))
                visibleEntity.Draw(spriteBatch);
        }
    }
}