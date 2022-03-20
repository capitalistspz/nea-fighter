using System;
using System.Collections.Generic;
using System.Linq;
using client.command;
using client.entities;
using client.graphics;
using client.input;
using common.command;
using common.core;
using common.entities;
using common.events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Tiled;

namespace client.core
{
    public partial class ClientGame : Game, IGame
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TextInputManager _textInputManager;
        private SplitscreenManager _splitscreenManager;
        
        // Where entities and collisions are handled 
        private ClientWorld _world;
        
        // Input device managers
        private KeyboardGameplayInputManager _keyboardGameplayInputManager;
        private KeyedCollection<int, ControllerGameplayInputManager> _controllerGameInputs;
        private ushort _maxControllers;
        
        private List<ClientPlayerEntity> _localPlayers;
        private ushort _localPlayerId;
        public ClientGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
               
        protected override void Initialize()
        {
            var res = Utils.GetMaxSupportedResolution(GraphicsAdapter.DefaultAdapter);
            _graphics.PreferredBackBufferWidth = res.X;
            _graphics.PreferredBackBufferHeight = res.Y;
            _graphics.ApplyChanges();
            _splitscreenManager = new SplitscreenManager(new Rectangle(0,0, res.X, res.Y));
            
            _localPlayerId = 0;
            _maxControllers = 4;
            _controllerGameInputs = new KeyedCollection<int, ControllerGameplayInputManager>(c=> c.ControllerId);
            _localPlayers = new List<ClientPlayerEntity>();
            
            _textInputManager = new TextInputManager(this);
            _textInputManager.Start();
            
            var tiledMap = Content.Load<TiledMap>("maps/default");
            _world = new ClientWorld(tiledMap, GraphicsDevice);
            
            SetCommands();
            InitNetwork();
            base.Initialize();
        }

        private void SetCommands()
        {
            HelpCommand.Register();
            ConnectCommand.Register();
        }
        
        /// <summary>
        /// Load graphical content
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LoadTextures("textures");
            base.LoadContent();
        }
        
        private void LoadTextures(string textureRoot)
        {
            var playerTexture = Content.Load<Texture2D>(textureRoot + "/entities/player");
            PlayerEntity.PlayerTexture = playerTexture;
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (_pollNetwork)
                _client.PollEvents();
            
            _pollNetwork = !_pollNetwork;
            
            PollInputs(gameTime);
            _world.Update(gameTime);
            _textInputManager.Update();
            base.Update(gameTime);
        }
        
        // Render everything
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            for (var i = 0; i < _localPlayers.Count; i++)
            {
                GraphicsDevice.Viewport = _splitscreenManager.GetView(i);
                
                var selectedPlayer = _localPlayers[i];

                selectedPlayer.Camera.LookAt(selectedPlayer.Position);
                
                _spriteBatch.Begin(transformMatrix:selectedPlayer.Camera.GetViewMatrix());
                _world.Draw(gameTime, _spriteBatch, selectedPlayer.Camera.GetViewMatrix());
                
                _spriteBatch.End();
            }
            
            base.Draw(gameTime);
        }
        
        public World GetWorld()
        {
            return _world;
        }

        public new void Exit()
        {
            _server?.Disconnect();
            _client.Stop();
            base.Exit();
        }
        
        // Checks for any user input
        private void PollInputs(GameTime gameTime)
        {
            // Spawns keyboard player on key press
            if (_keyboardGameplayInputManager == null)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    AddKeyboardPlayer();
            }
            else
            {
                _keyboardGameplayInputManager.Update(gameTime);
            }
            // Get the list of controllers that are connected
            var connectedGamepadIds = GetConnectedControllers();
            
            // Remove all controller ids already in use by the game
            connectedGamepadIds.RemoveAll(i => _controllerGameInputs.ContainsKey(i));
            
            // Create new player with controller if the controller presses start 
            foreach (var id in connectedGamepadIds.Where(id => GamePad.GetState(id).IsButtonDown(Buttons.Start)))
            {
                AddControllerPlayer(id);
            }
            foreach (var controller in _controllerGameInputs)
            {
                controller.Update(gameTime);
            }
            GameEvents.TriggerInputEvents();
        }
        
        public void AddKeyboardPlayer()
        {
            var newPlayer = new ClientPlayerEntity(_world, Guid.NewGuid(), _world.GetNewSpawn(), _localPlayerId++)
                {
                    Camera = new OrthographicCamera(GraphicsDevice)
                };

            _keyboardGameplayInputManager = new KeyboardGameplayInputManager(newPlayer);
            
            AddNewPlayer(newPlayer);
        }
        public void AddControllerPlayer(int padId)
        {
            var newPlayer = new ClientPlayerEntity(_world, Guid.NewGuid(), _world.GetNewSpawn(), _localPlayerId++)
                {
                    Camera = new OrthographicCamera(GraphicsDevice)
                };
            
            _controllerGameInputs.Add(new ControllerGameplayInputManager(newPlayer, padId));
            AddNewPlayer(newPlayer);
        }

        private void AddNewPlayer(ClientPlayerEntity newPlayer)
        {
            _splitscreenManager.IncrementViewCount();
            _localPlayers.Add(newPlayer);
            _world.AddEntity(newPlayer);
            SendLocalPlayerUpdate(newPlayer.LocalPlayerID, false);
        }

        private List<int> GetConnectedControllers()
        {
            var connectedControllerIds = new List<int>();
            for (var i = 0; i < _maxControllers; ++i)
            {
                if (GamePad.GetState(i).IsConnected)
                    connectedControllerIds.Add(i);
                
            }

            return connectedControllerIds;
        }
    }
}