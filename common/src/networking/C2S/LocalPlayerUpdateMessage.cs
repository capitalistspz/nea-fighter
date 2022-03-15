using LiteNetLib.Utils;

namespace common.networking.C2S
{
    public class LocalPlayerUpdateMessage : INetSerializable
    {
        private bool _removed;
        private ushort _localPlayerId;

        public LocalPlayerUpdateMessage()
        {
            _removed = false;
            _localPlayerId = ushort.MaxValue;
        }

        public LocalPlayerUpdateMessage(ushort localPlayerId, bool removed)
        {
            _localPlayerId = localPlayerId;
            _removed = removed;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)C2SMessageType.LocalPlayerUpdate);
            writer.Put(_localPlayerId);
            writer.Put(_removed);
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            _localPlayerId = reader.GetUShort();
            _removed = reader.GetBool();
        }

        public bool WasRemoved => _removed;
        public ushort LocalPlayerId => _localPlayerId;
    }
}