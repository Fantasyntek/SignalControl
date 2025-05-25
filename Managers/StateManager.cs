using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SignalControl.States;
using System;

namespace SignalControl.Managers
{
    public class StateManager
    {
        private GameState _currentState;

        public GameState CurrentState => _currentState;

        public void ChangeState(GameState newState)
        {
            _currentState?.UnloadContent();
            _currentState = newState;
            _currentState.LoadContent();
        }

        public void Update(GameTime gameTime)
        {
            _currentState?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentState?.Draw(spriteBatch);
        }
    }
} 