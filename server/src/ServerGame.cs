using System;
using System.Net;
using common.command;
using common.core;
using common.networking.C2S;
using common.networking.S2C;
using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;
using Serilog;
using server.entities;

namespace server
{
    public class ServerGame : Game , IGame
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextInputManager _textInputManager;
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
            _graphics.PreferredBackBufferHeight = 10;
            _graphics.PreferredBackBufferWidth = 10;
            _graphics.ApplyChanges();
            
            _textInputManager = new TextInputManager(this);
            _textInputManager.Start();
            
            InitNetwork();
            SetCommands();
            base.Initialize();
        }

        private void InitNetwork()
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener);
            _server.Start(IPAddress.Loopback, IPAddress.IPv6None, 35203);
            _listener.NetworkReceiveEvent += OnReceiveData;
            _listener.PeerConnectedEvent += OnConnect;
            _listener.ConnectionRequestEvent += OnConnectRequest;
            _listener.PeerDisconnectedEvent += OnDisconnect;
            _playerManager = new PlayerManager();
            Log.Information("Server initialised");
        }
        private void SetCommands()
        {
            HelpCommand.Register();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _server.PollEvents();
            _world.Update(gameTime);
            base.Update(gameTime);
        }
        
        private void OnConnectRequest(ConnectionRequest request)
        {
            Log.Information("Received a connection request from {RequestSource}", request.RemoteEndPoint);
            request.Accept();
        }

        private void OnDisconnect(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Log.Information("User(s) at {Peer} disconnected, reason: {DisconnectReason}", peer.EndPoint, disconnectInfo.Reason);
            var disconnected = _playerManager.GetPlayersByNetId(peer.Id);
            foreach (var player in disconnected)
            {
                _world.RemoveEntity(player);
                _playerManager.RemovePlayer(player);
            }
            
        }
        private void OnConnect(NetPeer peer)
        {
            Log.Information("User connected from {PeerEndpoint}", peer.EndPoint);
            
            // Send all existing entities to the new peer
            var entities = _world.GetAllEntities();
            var writer = new NetDataWriter();
            foreach (var entity in entities)
            {
                var msg = new EntityCreatedMessage(entity);
                writer.Put(msg);
                peer.Send(writer, DeliveryMethod.ReliableUnordered);
                writer.Reset();
            }
        }

        private void OnReceiveData(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            
            var type = (C2SMessageType) reader.PeekByte();
            Log.Debug("Received data message of type {MsgType}", type);
            switch (type)
            {
                case C2SMessageType.Input:
                {
                    var inputMsg = reader.Get<InputMessage>();
                    var inputArgs = inputMsg.InputArgs;
                    var player = _playerManager.GetPlayer(peer.Id, inputArgs.LocalPlayerId);
                    player.ApplyInput(inputMsg.InputArgs);
                    var writer = new NetDataWriter();
                    writer.Put(new EntityMotionMessage(player));
                    _server.SendToAll(writer, DeliveryMethod.ReliableSequenced);
                }
                    break;
                case C2SMessageType.ChatMessage:
                    break;
                case C2SMessageType.LocalPlayerUpdate:
                {
                    var localUpdateMsg = reader.Get<LocalPlayerUpdateMessage>();
                    if (!localUpdateMsg.WasRemoved)
                    {
                        var newLocalPlayer = new ServerPlayerEntity(_world, Guid.NewGuid(), _world.GetNewSpawn(), localUpdateMsg.LocalPlayerId, peer.Id);
                        
                        _playerManager.AddPlayer(newLocalPlayer);
                        _world.AddEntity(newLocalPlayer);
                        
                        var writer = new NetDataWriter();
                        writer.Put(new EntityCreatedMessage(newLocalPlayer));
                        _server.SendToAll(writer, DeliveryMethod.ReliableOrdered, peer);
                        
                        writer.Reset();
                        writer.Put(new AssignIdMessage(newLocalPlayer));
                        peer.Send(writer, DeliveryMethod.ReliableOrdered);
                    }
                    else
                    {
                        var removedLocalPlayer = _playerManager.GetPlayer(peer.Id, localUpdateMsg.LocalPlayerId);
                        _playerManager.RemovePlayer(removedLocalPlayer);
                        _world.RemoveEntity(removedLocalPlayer);
                        var writer = new NetDataWriter();
                        writer.Put(new EntityRemovedMessage(removedLocalPlayer.Id));
                    }
                }
                    break;
                default:
                    Log.Warning("Unknown data message type received");
                    break;
                
            }
        }

        protected new void Exit()
        {
            _server.DisconnectAll();
            _server.Stop();
            base.Exit();
        }

        public World GetWorld()
        {
            return _world;
        }
    }
}