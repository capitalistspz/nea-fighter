using System;
using common.entities;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace common.networking.S2C
{
    public class EntityCreatedMessage : INetSerializable
    {
        private Guid _id;
        private EntityType _entityType;
        private Vector2 _entityPosition;

        public EntityCreatedMessage()
        {
            _entityType = EntityType.Unknown;
            _id = Guid.Empty;
            _entityPosition = Vector2.Zero;
        }
        public EntityCreatedMessage(BaseEntity entity)
        {
            switch (entity)
            {
                case PlayerEntity:
                    _entityType = EntityType.PlayerEntity;
                    break;
                default:
                    _entityType = EntityType.Unknown;
                    break;
            }

            _entityPosition = entity.Position;
            _id = entity.Id;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)S2CMessageType.EntityCreated);
            writer.Put((byte)_entityType);
            writer.Put(_entityPosition.X);
            writer.Put(_entityPosition.Y);
            writer.Put(_id.ToByteArray());
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            _entityType = (EntityType) reader.GetByte();
            _entityPosition.X = reader.GetFloat();
            _entityPosition.Y = reader.GetFloat();
            
            // Guid is made of 16 bytes
            var bytes = new byte[16];
            reader.GetBytes(bytes, 16);
            _id = new Guid(bytes);
        }

        public Guid Id => _id;
        public EntityType Type => _entityType;
        public Vector2 Position => _entityPosition;
    }
}