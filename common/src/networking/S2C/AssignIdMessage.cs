using System;
using common.entities;
using LiteNetLib.Utils;

namespace common.networking.S2C
{
    public class AssignIdMessage : INetSerializable
    {
        private Guid _id;
        private ushort _localPlayerId;

        public AssignIdMessage()
        {
            _id = Guid.Empty;
            _localPlayerId = UInt16.MaxValue;
        }

        public AssignIdMessage(PlayerEntity player)
        {
            _id = player.Id;
            _localPlayerId = player.LocalPlayerID;
        }
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)S2CMessageType.AssignId);
            writer.Put(_localPlayerId);
            writer.Put(_id.ToByteArray());
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            _localPlayerId = reader.GetUShort();
            var bytes = new byte[16];
            reader.GetBytes(bytes, 16);
            _id = new Guid(bytes);
        }

        public Guid Id => _id;
        public ushort LocalPlayerId => _localPlayerId;
    }
}