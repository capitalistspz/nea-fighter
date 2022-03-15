using common.events;
using common.physics;
using LiteNetLib.Utils;

namespace common.networking.C2S
{
    // Sent when keys/buttons are pressed in gameplay
    public class InputMessage : INetSerializable
    {
        private InputEventArgs _args;

        public InputMessage(InputEventArgs eventArgs)
        {
            _args = eventArgs;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)C2SMessageType.Input);
            writer.Put(_args.LocalPlayerId);
            writer.Put(_args.MovementDirection);
            writer.Put(_args.AimDirection.X);
            writer.Put(_args.AimDirection.Y);
            writer.Put(_args.Actions.GetByte());
        }

        public void Deserialize(NetDataReader reader)
        {
            _ = reader.GetByte();
            _args.LocalPlayerId = reader.GetUShort();
            _args.MovementDirection = reader.Get<Vector2I>();
            _args.AimDirection.X = reader.GetFloat();
            _args.AimDirection.Y = reader.GetFloat();
            _args.Actions.SetByte(reader.GetByte());
        }
    }
}