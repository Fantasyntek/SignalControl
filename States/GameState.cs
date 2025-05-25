using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SignalControl.Managers;

namespace SignalControl.States
{
    public abstract class GameState
    {
        protected Game _game;
        protected StateManager _stateManager;
        protected ContentManager _content;

        public GameState(Game game, StateManager stateManager, ContentManager content)
        {
            _game = game;
            _stateManager = stateManager;
            _content = content;
        }

        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
} 