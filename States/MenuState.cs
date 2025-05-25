using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SignalControl.Managers;
using SignalControl.UI;

namespace SignalControl.States
{
    public class MenuState : GameState
    {
        private List<Button> _buttons;
        private MouseState _previousMouseState;
        private string _title = "Контроль Сигнала";
        private float _animTime = 0;
        
        // Константы для дизайна
        private readonly Color _backgroundColor = new Color((byte)20, (byte)20, (byte)40);
        private readonly Color _titleColor = new Color((byte)100, (byte)200, (byte)255);
        private readonly Color _textColor = new Color((byte)220, (byte)220, (byte)255);
        
        public MenuState(Game game, StateManager stateManager, ContentManager content) 
            : base(game, stateManager, content)
        {
            _buttons = new List<Button>();
        }

        public override void LoadContent()
        {
            // Создаем кнопки
            Vector2 position = new Vector2(640, 320);
            _buttons.Add(new Button("Начать игру", position, () => 
            {
                _stateManager.ChangeState(new GameplayState(_game, _stateManager, _content, 0));
            }));
            
            position.Y += 80;
            _buttons.Add(new Button("Выбор уровня", position, () => 
            {
                _stateManager.ChangeState(new LevelSelectState(_game, _stateManager, _content));
            }));
            
            position.Y += 80;
            _buttons.Add(new Button("Выход", position, () => 
            {
                _game.Exit();
            }));
        }

        public override void Update(GameTime gameTime)
        {
            // Обновляем время для анимаций
            _animTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            MouseState currentMouseState = Mouse.GetState();
            
            foreach (var button in _buttons)
            {
                button.Update(currentMouseState);
                
                if (currentMouseState.LeftButton == ButtonState.Released && 
                    _previousMouseState.LeftButton == ButtonState.Pressed &&
                    button.IsHovering)
                {
                    button.Click();
                }
            }
            
            _previousMouseState = currentMouseState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Рисуем фон
            DrawBackground(spriteBatch);
            
            // Рисуем заголовок
            DrawTitle(spriteBatch);
            
            // Рисуем кнопки
            foreach (var button in _buttons)
            {
                button.Draw(spriteBatch, null);
            }
            
            // Рисуем инструкции
            DrawInstructions(spriteBatch);
            
            // Рисуем информацию об авторе
            DrawAuthorInfo(spriteBatch);
        }
        
        private void DrawBackground(SpriteBatch spriteBatch)
        {
            // Создаем фоновую текстуру
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Рисуем основной фон
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height),
                _backgroundColor
            );
            
            // Рисуем декоративные элементы
            for (int i = 0; i < 100; i++)
            {
                // Создаем пульсирующие точки на фоне
                float x = (float)Math.Sin(_animTime * 0.5f + i * 0.1f) * 600 + 640;
                float y = (float)Math.Cos(_animTime * 0.7f + i * 0.2f) * 300 + 360;
                
                // Размер точки зависит от синуса
                int size = (int)((Math.Sin(_animTime + i) + 1) * 2) + 1;
                
                // Цвет с постепенно изменяющейся прозрачностью
                byte alpha = (byte)((Math.Sin(_animTime * 2 + i) + 1) * 40);
                Color dotColor = new Color((byte)100, (byte)150, (byte)255, alpha);
                
                spriteBatch.Draw(
                    pixel,
                    new Rectangle((int)x, (int)y, size, size),
                    dotColor
                );
            }
            
            // Рисуем декоративные линии
            for (int i = 0; i < 5; i++)
            {
                float y = 100 + i * 120;
                float offset = (float)Math.Sin(_animTime * 0.5f + i * 0.7f) * 50;
                
                DrawLine(
                    spriteBatch, 
                    new Vector2(0, y + offset), 
                    new Vector2(_game.GraphicsDevice.Viewport.Width, y - offset), 
                    new Color((byte)50, (byte)100, (byte)200, (byte)30),
                    1
                );
            }
        }
        
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            spriteBatch.Draw(
                pixel,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0
            );
        }
        
        private void DrawTitle(SpriteBatch spriteBatch)
        {
            // Создаем эффект "пульсации" для заголовка
            float scale = 2.5f + (float)Math.Sin(_animTime * 2) * 0.1f;
            
            // Рисуем основной заголовок
            TextRenderer.DrawText(
                spriteBatch,
                _title,
                new Vector2(640 - _title.Length * 7 * scale/2, 150),
                _titleColor,
                scale
            );
            
            // Рисуем тень заголовка для эффекта
            TextRenderer.DrawText(
                spriteBatch,
                _title,
                new Vector2(643 - _title.Length * 7 * scale/2, 153),
                new Color((byte)50, (byte)50, (byte)150),
                scale
            );
            
            // Подзаголовок
            string subtitle = "Логическая головоломка";
            TextRenderer.DrawText(
                spriteBatch,
                subtitle,
                new Vector2(640 - subtitle.Length * 5, 230),
                _textColor,
                1.2f
            );
        }
        
        private void DrawInstructions(SpriteBatch spriteBatch)
        {
            string controls = "Управление: используйте мышь для выбора и поворота узлов";
            Vector2 textSize = TextRenderer.MeasureString(controls, 1.0f);
            TextRenderer.DrawText(
                spriteBatch,
                controls,
                new Vector2(640 - textSize.X / 2, 500),
                _textColor,
                1.0f
            );
        }
        
        private void DrawAuthorInfo(SpriteBatch spriteBatch)
        {
            string version = "Версия 1.0";
            TextRenderer.DrawText(
                spriteBatch,
                version,
                new Vector2(20, _game.GraphicsDevice.Viewport.Height - 30),
                _textColor,
                0.8f
            );
            
            string copyright = "(c) 2025 Signal Control";
            TextRenderer.DrawText(
                spriteBatch,
                copyright,
                new Vector2(_game.GraphicsDevice.Viewport.Width - copyright.Length * 10, _game.GraphicsDevice.Viewport.Height - 30),
                _textColor,
                0.8f
            );
        }
    }
} 