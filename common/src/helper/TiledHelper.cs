using System.Collections.Generic;
using System.Linq;
using common.physics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace common.helper
{
    public static class TiledHelper
    {
        // Generates basic collision objects from collision layer objects
        public static List<BasicCollisionObject> CreateCollisionsFromMap(TiledMap map, string collisionLayerName)
        {
            var collisionObjects = new List<BasicCollisionObject>();
            var layer = map.ObjectLayers.Single(objectLayer => objectLayer.Name == collisionLayerName);
            if (layer != null)
                collisionObjects.AddRange(layer.Objects.Select(mapObject => new BasicCollisionObject(new RectangleF(mapObject.Position, mapObject.Size))));
            return collisionObjects;
        }
    }
}