using System;
using System.Collections.Generic;
using System.Linq;
using common.entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using Serilog;

namespace common.core
{
    // Managers updates and removal of entities
    public class EntityManager
    {
        private KeyedCollection<Guid, BaseEntity> _entities;
        
        // For controlled timing of removals and additions
        private Queue<BaseEntity> _newEntities;
        private Queue<BaseEntity> _oldEntities;
        public EntityManager()
        {
            _entities = new KeyedCollection<Guid,BaseEntity>(e => e.Id);
            _newEntities = new Queue<BaseEntity>();
            _oldEntities = new Queue<BaseEntity>();
        }

        public void Update(GameTime time)
        {
            while (_oldEntities.Count > 0)
            {
                _entities.Remove(_oldEntities.Dequeue());
            }
            foreach (var entity in _entities.Values)
            {
                entity.Update(time);
            }
            while (_newEntities.Count > 0)
            {
                _entities.Add(_newEntities.Dequeue());
            }
            
        }

        public void RemoveEntity(BaseEntity entity)
        {
            _oldEntities.Enqueue(entity);
            Log.Debug("Entity of type {EntityType} removed", entity.GetType());
        }

        public void AddEntity(BaseEntity entity)
        {
            _newEntities.Enqueue(entity);
            Log.Debug("Entity of type {EntityType} added at position {EntityPosition}", entity.GetType(), entity.Position);
        }
        public BaseEntity GetEntity(Guid id)
        {
            return _entities[id];
        }
        
        public PlayerEntity? GetPlayer(Guid id)
        {
            var entity = _entities[id];
            if (entity is PlayerEntity player)
                return player;
            return null;
        }

        public IEnumerable<BaseEntity> FilterEntities(Predicate<BaseEntity> conditions)
        {
            return _entities.Values.Where(conditions.Invoke);
        }
        public IEnumerable<BaseEntity> GetAllEntities()
        {
            return _entities.Values;
        }
    }
}