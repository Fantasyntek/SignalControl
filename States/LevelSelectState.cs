using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SignalControl.Managers;
using SignalControl.UI;
using System;
using System.Collections.Generic;

namespace SignalControl.States
{
    public class LevelSelectState : GameState
    {
        private List<Button> _levelButtons;
        private Button _backButton;
        private MouseState _previousMouseState;
        private LevelManager _levelManager;
        private float _animTime = 0;
        
        // Константы для дизайна
        private readonly Color _backgroundColor = new Color((byte)20, (byte)20, (byte)40);
        private readonly Color _panelColor = new Color((byte)50, (byte)50, (byte)80);
        private readonly Color _titleColor = new Color((byte)100, (byte)200, (byte)255);
        private readonly Color _textColor = new Color((byte)220, (byte)220, (byte)255);
        
        public LevelSelectState(Game game, StateManager stateManager, ContentManager content) 
            : base(game, stateManager, content)
        {
            _levelButtons = new List<Button>();
            _levelManager = new LevelManager();
        }
        
        public override void LoadContent()
        {
            // Определение размеров экрана для правильного позиционирования
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            float centerX = screenWidth / 2;
            
            // Создаем кнопки для каждого уровня
            int levelsCount = _levelManager.GetLevelsCount();
            int buttonsPerRow = 3;
            int buttonWidth = 200;
            int buttonHeight = 200;
            // Вычисляем позиции относительно центра экрана
            int startX = (int)centerX - (Math.Min(buttonsPerRow, levelsCount) * buttonWidth) / 2 + buttonWidth/2;
            int startY = 250;
            
            for (int i = 0; i < levelsCount; i++)
            {
                int row = i / buttonsPerRow;
                int col = i % buttonsPerRow;
                
                int x = startX + col * buttonWidth;
                int y = startY + row * buttonHeight;
                
                int levelIndex = i; // Capture for lambda
                _levelButtons.Add(new Button($"Уровень {i + 1}", new Vector2(x, y), () => 
                {
                    _stateManager.ChangeState(new GameplayState(_game, _stateManager, _content, levelIndex));
                }));
            }
            
            // Кнопка возврата в меню (внизу по центру)
            _backButton = new Button("Назад", new Vector2(centerX, screenHeight - 100), () => 
            {
                _stateManager.ChangeState(new MenuState(_game, _stateManager, _content));
            });
        }
        
        public override void Update(GameTime gameTime)
        {
            _animTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            MouseState currentMouseState = Mouse.GetState();
            
            // Обновляем все кнопки уровней
            foreach (var button in _levelButtons)
            {
                button.Update(currentMouseState);
            }
            
            // Обновляем кнопку назад
            _backButton.Update(currentMouseState);
            
            // Проверяем клики
            if (currentMouseState.LeftButton == ButtonState.Released && 
                _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                foreach (var button in _levelButtons)
                {
                    if (button.IsHovering)
                    {
                        button.Click();
                        break;
                    }
                }
                
                if (_backButton.IsHovering)
                {
                    _backButton.Click();
                }
            }
            
            _previousMouseState = currentMouseState;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Рисуем фон
            DrawBackground(spriteBatch);
            
            // Рисуем заголовок с учетом центрирования
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            string title = "Выбор уровня";
            TextRenderer.DrawText(
                spriteBatch,
                title,
                new Vector2(screenWidth / 2 - TextRenderer.MeasureString(title, 2.0f).X / 2, 100),
                _titleColor,
                2.0f
            );
            
            // Рисуем кнопки уровней
            foreach (var button in _levelButtons)
            {
                button.Draw(spriteBatch, null);
            }
            
            // Рисуем кнопку назад
            _backButton.Draw(spriteBatch, null);
        }
        
        private void DrawBackground(SpriteBatch spriteBatch)
        {
            // Получаем размеры экрана для адаптивного UI
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            
            // Создаем фоновую текстуру
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Рисуем основной фон
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, 0, (int)screenWidth, (int)screenHeight),
                _backgroundColor
            );
            
            // Вычисляем размер и позицию панели относительно размера экрана
            int panelWidth = Math.Min(1000, (int)(screenWidth * 0.8f));
            int panelHeight = Math.Min(800, (int)(screenHeight * 0.85f));
            int panelX = (int)(screenWidth / 2 - panelWidth / 2);
            int panelY = (int)(screenHeight / 2 - panelHeight / 2);
            
            // Рисуем декоративную панель
            Rectangle panelRect = new Rectangle(panelX, panelY, panelWidth, panelHeight);
            spriteBatch.Draw(pixel, panelRect, _panelColor);
            
            // Рисуем рамку панели
            DrawRectangleOutline(spriteBatch, panelRect, _titleColor, 2);
        }
        
        private void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Верхняя линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), 
                color
            );
            
            // Нижняя линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), 
                color
            );
            
            // Левая линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), 
                color
            );
            
            // Правая линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), 
                color
            );
        }
    }
} 