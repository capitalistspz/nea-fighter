using System;
using System.Collections.Generic;
using client.entities;
using client.input;
using common.core;
using common.entities;
using common.helper;
using LiteNetLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace client.core
{
    public partial class ClientGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
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
        
        /// <summary>
        /// Load non graphical resources
        /// </summary>
        protected override void Initialize()
        {
            _localPlayerId = 0;
            _maxControllers = 4;
            _controllerGameInputs = new List<ControllerGameplayInput>();
            _localPlayers = new List<ClientPlayerEntity>();
            var tiledMap = Content.Load<TiledMap>("maps/default");
            _world = new ClientWorld(tiledMap, GraphicsDevice);
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
            PollInputs(gameTime);
            _world.Update(gameTime);
            base.Update(gameTime);
        }
        
        // Render everything
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();
            // Draw from here
            _world.Draw(gameTime, _spriteBatch, mapTransform: Matrix.Identity);
            // To here 
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        
        public World GetWorld()
        {
            return _world;
        }

        public new void Exit()
        {
            base.Exit();
        }
        
        // Checks for any user input
        public void PollInputs(GameTime gameTime)
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
            
            foreach (var controller in _controllerGameInputs)
            {
                controller.Update(gameTime);
            }
        }
        
        public void AddKeyboardPlayer()
        {
            var newPlayer = new ClientPlayerEntity(_world, Guid.NewGuid(), Vector2.One, _localPlayerId++);
            _keyboardGameplayInput = new KeyboardGameplayInput(newPlayer);
            _localPlayers.Add(newPlayer);
            _world.AddEntity(newPlayer);
        }
        public void AddControllerPlayer(int padId)
        {
            var newPlayer = new ClientPlayerEntity(_world, Guid.NewGuid(), Vector2.One, _localPlayerId++);
            _controllerGameInputs.Add(new ControllerGameplayInput(newPlayer, padId));
            _localPlayers.Add(newPlayer);
            _world.AddEntity(newPlayer);
            SendLocalPlayerUpdate(newPlayer.LocalPlayerID, false);
        }
    }
}