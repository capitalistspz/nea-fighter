using System;
using System.Collections.Concurrent;
using System.Net;
using common.entities;
using common.events;
using common.networking.C2S;
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
        private bool _pollNetwork;
        public ConcurrentQueue<INetSerializable> MessageQueue;
        
        public bool NetConnected { get; private set; }
        protected void InitNetwork()
        {
            _listener = new EventBasedNetListener();
            _client = new NetManager(_listener);
            _listener.NetworkReceiveEvent += OnReceiveData;
            _pollNetwork = false;
            NetConnected = false;
            MessageQueue = new ConcurrentQueue<INetSerializable>();

            GameEvents.InputEvent += OnInput;
        }
        // Connect to a server
        public void Connect(IPAddress ipv4Address, int port, string serverPassword)
        {
            var endpoint = new IPEndPoint(ipv4Address, port);
            _client.Start();
            _server = _client.Connect(endpoint, serverPassword);
            NetConnected = true;
            Log.Debug("Networking initialised");
        }
        // Disconnect from a server
        public void Disconnect()
        {
            _server.Disconnect();
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
                    NetMoveEntity(reader.Get<EntityMotionMessage>());
                    break;
                case S2CMessageType.EntityRemoved:
                    NetRemoveEntity(reader.Get<EntityRemovedMessage>());
                    break;
                case S2CMessageType.TextMessage:
                    break;
                case S2CMessageType.AssignId:
                    NetAssignId(reader.Get<AssignIdMessage>());
                    break;
                default:
                    Log.Warning("Received unknown message type: {Type}", type);
                    break;
            }
        }

        private void NetAssignId(AssignIdMessage msg)
        {
            Log.Debug("Assigning Id {Id} to local user {LocalId}", msg.Id, msg.LocalPlayerId);
            var player = _localPlayers.Find(e => e.LocalPlayerID == msg.LocalPlayerId);
            if (player != null)
            {
                // Effectively refreshes the player in the dictionary
                player.Id = msg.Id;
                _world.RemoveEntity(player);
                _world.AddEntity(player);
            }
            else
            {
                Log.Warning("Unable to assign Id to local user");
                ListPlayerIds();
            }
            
        }

        private void ListPlayerIds()
        {
            foreach (var playerEntity in _localPlayers)
            {
                Log.Debug("Identities {LocalId} : {Guid}",playerEntity.LocalPlayerID, playerEntity.Id);
            }
        }
        private void NetMoveEntity(EntityMotionMessage msg)
        {
            var entity = _world.GetEntity(msg.Id);
            if (entity == null)
            {
                Log.Warning("Attempted to move entity, but entity not found");
                return;
            }
            entity.AssignServerMotion(msg.Position, msg.Velocity);
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
        
        private void OnInput(object sender, InputEventArgs args)
        {
            var msg = new InputMessage(args);
            var writer = new NetDataWriter();
            writer.Put(msg);
            _server?.Send(writer, DeliveryMethod.ReliableOrdered);
        }
        
        // Tells the server when a local player is added or removed
        private void SendLocalPlayerUpdate(ushort localPlayerId, bool removed)
        {
            var writer = new NetDataWriter();
            writer.Put(new LocalPlayerUpdateMessage(localPlayerId, removed));
            _server?.Send(writer, DeliveryMethod.ReliableOrdered);
        }
    }
}