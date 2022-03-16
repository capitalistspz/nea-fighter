using System;
using System.Collections.Generic;
using client.entities;
using client.graphics;
using client.input;
using common.core;
using common.entities;
using common.events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;

namespace client.core
{
    public partial class ClientGame : Game, IGame
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private SplitscreenManager _splitscreenManager;
        
        // Where entities and collisions are handled 
        private ClientWorld _world;
        
        // Input device managers
        private KeyboardGameplayInput _keyboardGameplayInput;
        private List<ControllerGameplayInput> _controllerGameInputs;
        private ushort _maxControllers;
        
        private List<ClientPlayerEntity> _localPlayers;
        private ushort _localPlayerId;
        public ClientGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
        }
        
        protected override void Initialize()
        {
            _localPlayerId = 0;
            _maxControllers = 4;
            _controllerGameInputs = new List<ControllerGameplayInput>();
            _localPlayers = new List<ClientPlayerEntity>();
            var tiledMap = Content.Load<TiledMap>("maps/default");
            _world = new ClientWorld(tiledMap, GraphicsDevice);
            InitNetwork();
            
            {
                var width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                var height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                _splitscreenManager = new SplitscreenManager(new Rectangle(0,0, width, height));
            }
            
            base.Initialize();
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
            if (_keyboardGameplayInput == null)
            {
                if (Keyboard.GetState().GetPressedKeyCount() > 0)
                    AddKeyboardPlayer();
            }
            else
            {
                _keyboardGameplayInput.Update(gameTime);
            }

            if (_controllerGameInputs.Count == 0)
            {
                if (GamePad.GetState(0).IsButtonDown(Buttons.Start))
                    AddControllerPlayer(0);
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

            _keyboardGameplayInput = new KeyboardGameplayInput(newPlayer);
            
            AddNewPlayer(newPlayer);
        }
        public void AddControllerPlayer(int padId)
        {
            var newPlayer = new ClientPlayerEntity(_world, Guid.NewGuid(), _world.GetNewSpawn(), _localPlayerId++)
                {
                    Camera = new OrthographicCamera(GraphicsDevice)
                };
            
            _controllerGameInputs.Add(new ControllerGameplayInput(newPlayer, padId));
            AddNewPlayer(newPlayer);
        }

        private void AddNewPlayer(ClientPlayerEntity newPlayer)
        {
            _splitscreenManager.IncrementViewCount();
            _localPlayers.Add(newPlayer);
            _world.AddEntity(newPlayer);
            SendLocalPlayerUpdate(newPlayer.LocalPlayerID, false);
        }
    }
}