using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SignalControl.States;
using SignalControl.Managers;
using SignalControl.UI;
using System.IO;
using System;

namespace SignalControl
{
    public class SignalControlGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private StateManager _stateManager;
        private Color _backgroundColor = new Color((byte)20, (byte)20, (byte)40); // Темно-синий фон

        public SignalControlGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            
            _stateManager = new StateManager();
            
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            try
            {
                // Load the Arial SpriteFont
                var font = Content.Load<SpriteFont>("Arial");
                TextRenderer.Initialize(font);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading font: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
            
            // Initialize TextureManager
            TextureManager.Initialize(GraphicsDevice, Content);
            
            // Initialize first state
            _stateManager.ChangeState(new MenuState(this, _stateManager, Content));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _stateManager.CurrentState?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);
            
            // Используем правильные настройки для SpriteBatch
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone
            );
            
            _stateManager.CurrentState?.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}