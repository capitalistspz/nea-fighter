using System;
using common.entities;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace common.networking.S2C
{
    public class EntityMotionMessage : INetSerializable
    {
        private Guid _id;
        private Point2 _position;
        private Vector2 _velocity;

        public EntityMotionMessage()
        {
            _position = Point2.Zero;
            _velocity = Vector2.Zero;
            _id = Guid.Empty;
        }

        public EntityMotionMessage(BaseEntity entity)
        {
            _position = entity.Position;
            _velocity = entity.Velocity;
            _id = entity.Id;

        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)S2CMessageType.EntityMotion);
            writer.Put(_position.X);
            writer.Put(_position.Y);
            writer.Put(_velocity.X);
            writer.Put(_velocity.Y);
            writer.Put(_id.ToByteArray());
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            _position.X = reader.GetFloat();
            _position.Y = reader.GetFloat();
            _velocity.X = reader.GetFloat();
            _velocity.Y = reader.GetFloat();
            
            var bytes = new byte[16];
            reader.GetBytes(bytes, 16);
            _id = new Guid(bytes);
        }
    }
}