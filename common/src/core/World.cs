using System;
using System.Collections.Generic;
using common.entities;
using common.helper;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tiled;
using Serilog;

namespace common.core
{
    public class World
    {
        private EntityManager _entityManager;
        // For collision handling
        private CollisionComponent _collisionComponent;
        private TiledMap _tiledMap;
        private List<Vector2> _spawnPositions;
        private int _currentSpawn;
        public World(TiledMap map)
        {
            var bounds = new RectangleF(0, 0, map.Width * map.TileWidth, map.Height * map.TileHeight);
            _collisionComponent = new CollisionComponent(bounds);
            _entityManager = new EntityManager();
            _tiledMap = map;
            
            // Allows entities to collide with the map
            var collisionObjects = TiledHelper.CreateCollisionsFromMap(_tiledMap, "Collision Object Layer");
            foreach (var basicCollisionObject in collisionObjects)
            {
                _collisionComponent.Insert(basicCollisionObject);
                Log.Debug("Added map collision object {Bounds} ", basicCollisionObject.Bounds);
            }

            _spawnPositions = TiledHelper.GetSpawnPositionsFromMap(_tiledMap, "Player Spawn Layer");

        }
        public void AddEntity(BaseEntity entity)
        {
            _collisionComponent.Insert(entity);
            _entityManager.AddEntity(entity);
        }

        public Vector2 GetNewSpawn()
        {
            if (_spawnPositions.Count <= 0) 
                return Vector2.Zero;
            var p = _currentSpawn++;
            _currentSpawn %= _spawnPositions.Count;
            return _spawnPositions[p];
        }
        public IEnumerable<BaseEntity> FilterEntities(Predicate<BaseEntity> conditions)
        {
            return _entityManager.FilterEntities(conditions);
        }
        
        public void RemoveEntity(BaseEntity entity)
        {
            _collisionComponent.Remove(entity);
            _entityManager.RemoveEntity(entity);
            
        }

        public BaseEntity? GetEntity(Guid id)
        {
            return _entityManager.GetEntity(id);
        }

        public IEnumerable<BaseEntity> GetAllEntities()
        {
            return _entityManager.GetAllEntities();
        }
        public void RemoveCollisionObject(ICollisionActor actor)
        {
            _collisionComponent.Remove(actor);
        }

        public void Update(GameTime gameTime)
        {
            _entityManager.Update(gameTime);
            _collisionComponent.Update(gameTime);
        }
    }
    
}