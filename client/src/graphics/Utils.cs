using common.physics;
using Microsoft.Xna.Framework.Graphics;

namespace client.graphics
{
    public static class Utils
    {
        public static Vector2I GetMaxSupportedResolution(GraphicsAdapter graphicsAdapter)
        {
            var displayModes = graphicsAdapter.SupportedDisplayModes;
            var maxDisplayRes = Vector2I.Zero;
            foreach (var displayMode in displayModes)
            {
                if (displayMode.Width * displayMode.Height > maxDisplayRes.X * maxDisplayRes.Y)
                {
                    maxDisplayRes.X = displayMode.Width;
                    maxDisplayRes.Y = displayMode.Height;
                }
            }

            return maxDisplayRes;
        } 
    }
}