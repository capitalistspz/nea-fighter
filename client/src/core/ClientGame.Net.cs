using System.Collections.Concurrent;
using System.Net;
using common.entities;
using common.networking.S2C;
using LiteNetLib;
using LiteNetLib.Utils;
using Serilog;

namespace client.core
{
    public partial class ClientGame 
    {
        private NetManager _client;
        private NetPeer _server;
        private EventBasedNetListener _listener;
        public ConcurrentQueue<INetSerializable> MessageQueue;
        public bool NetConnected { get; private set; }
        protected void InitNetwork()
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _listener.NetworkReceiveEvent += OnReceiveData;
            NetConnected = false;
            MessageQueue = new ConcurrentQueue<INetSerializable>();
        }
        // Connect to a server
        public void Connect(IPAddress ipv4Address, int port, string serverPassword)
        {
            var endpoint = new IPEndPoint(ipv4Address, port);
            _client.Start();
            _server = _client.Connect(endpoint, serverPassword);
            NetConnected = true;
        }
        // Disconnect from a server
        public void Disconnect()
        {
            _client.Stop();
            NetConnected = false;

        }
        private void OnReceiveData(NetPeer peer, NetDataReader reader, DeliveryMethod deliveryMethod)
        {
            var type = (S2CMessageType) reader.PeekByte();
            switch (type)
            {
                case S2CMessageType.EntityCreated:
                    NetCreateEntity(reader.Get<EntityCreatedMessage>());
                    break;
                case S2CMessageType.EntityMotion:
                    break;
                case S2CMessageType.EntityRemoved:
                    NetRemoveEntity(reader.Get<EntityRemovedMessage>());
                    break;
                case S2CMessageType.TextMessage:
                    break;
                default:
                    Log.Warning("Received unknown message type: {Type}", type);
                    break;
            }
        }

        private void NetRemoveEntity(EntityRemovedMessage msg)
        {
            _world.RemoveEntity(_world.GetEntity(msg.Id));
        }
        
        private void NetCreateEntity(EntityCreatedMessage msg)
        {
            switch (msg.Type)
            {
                case EntityType.PlayerEntity:
                    _world.AddEntity(new PlayerEntity(_world, msg.Id, msg.Position));
                    break;
                case EntityType.ProjectileEntity:
                    // Projectile Entity does not exist yet
                    break;
                default:
                    Log.Warning("Received unknown entity type");
                    break;
            }
        }
    }
}