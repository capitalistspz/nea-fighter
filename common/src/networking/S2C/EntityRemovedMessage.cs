using System;
using LiteNetLib.Utils;

namespace common.networking.S2C
{
    public class EntityRemovedMessage : INetSerializable
    {
        private Guid _id;

        public EntityRemovedMessage()
        {
            _id = Guid.Empty;
        }
        public EntityRemovedMessage(Guid removedEntityId)
        {
            _id = removedEntityId;
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)S2CMessageType.EntityRemoved);
            writer.Put(_id.ToByteArray());
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            
            var bytes = new byte[16];
            reader.GetBytes(bytes, 16);
            _id = new Guid(bytes);
        }

        public Guid Id => _id;
    }
}