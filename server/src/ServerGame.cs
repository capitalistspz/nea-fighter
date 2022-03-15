using System;
using System.Net;
using common.core;
using common.networking.C2S;
using common.networking.S2C;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Serilog;
using server.entities;

namespace server
{
    public class ServerGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private EventBasedNetListener _listener;
        private NetManager _server;
        private PlayerManager _playerManager;
        private World _world;
        public ServerGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            
            base.Initialize();
        }

        protected void InitNetwork()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
            _server.Start(IPAddress.Loopback, IPAddress.IPv6None, 35203);
            _listener.NetworkReceiveEvent += OnReceiveData;
            _listener.PeerConnectedEvent += OnConnect;
            _playerManager = new PlayerManager();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        private void OnConnect(NetPeer peer)
        {
            var player = new ServerPlayerEntity(_world, Guid.NewGuid(), Vector2.Zero, 0);
            var outMsg = new EntityCreatedMessage(player);
            var writer = new NetDataWriter();
            writer.Put(outMsg);
            _server.SendToAll(writer, DeliveryMethod.ReliableOrdered);
            _playerManager.AddPlayer(player, peer.Id);
            _world.AddEntity(player);
        }

        private void OnReceiveData(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var type = (C2SMessageType) reader.PeekByte();
            switch (type)
            {
                case C2SMessageType.Input:
                    var inputMsg = reader.Get<InputMessage>();
                    var inputArgs = inputMsg.InputArgs;
                    var player = _playerManager.GetPlayer(peer.Id, inputArgs.LocalPlayerId);
                    player.ApplyInput(inputMsg.InputArgs);
                    break;
                case C2SMessageType.ChatMessage:
                    break;
                case C2SMessageType.LocalPlayerUpdate:
                    var localUpdateMsg = reader.Get<LocalPlayerUpdateMessage>();
                    if (!localUpdateMsg.WasRemoved)
                    {
                        var newLocalPlayer = new ServerPlayerEntity(_world, Guid.NewGuid(), Vector2.Zero, localUpdateMsg.LocalPlayerId);
                        _playerManager.AddPlayer(newLocalPlayer, peer.Id);
                        
                    }
                    else
                    {
                        var removedLocalPlayer = _playerManager.GetPlayer(peer.Id, localUpdateMsg.LocalPlayerId);
                        _playerManager.RemovePlayer(peer.Id, localUpdateMsg.LocalPlayerId);
                        _world.RemoveEntity(removedLocalPlayer);
                    }
                    break;
                default:
                    Log.Warning("Unknown data message type received");
                    break;
            }
        }
    }
}