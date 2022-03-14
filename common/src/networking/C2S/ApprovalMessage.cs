using LiteNetLib.Utils;

namespace common.networking
{
    // Sent on connection
    public class ApprovalMessage : INetSerializable
    {
        private string _serverPassword, _username;
        public ApprovalMessage(string serverPassword, string username)
        {
            _serverPassword = serverPassword;
            _username = username;
        }
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(_serverPassword);
            writer.Put(_username);
        }

        public void Deserialize(NetDataReader reader)
        {
            _serverPassword = reader.GetString();
            _username = reader.GetString();
        }
    }
}