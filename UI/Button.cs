using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace SignalControl.UI
{
    public class Button
    {
        private Rectangle _rectangle;
        private string _text;
        private Action _onClick;
        public bool IsHovering { get; private set; }
        
        private Color _normalColor = new Color((byte)30, (byte)30, (byte)60);     // Темно-синий
        private Color _hoverColor = new Color((byte)60, (byte)60, (byte)120);     // Светло-синий
        private Color _textColor = new Color((byte)220, (byte)220, (byte)255);    // Светлый текст
        private Color _borderColor = new Color((byte)100, (byte)100, (byte)200);  // Синяя рамка
        
        public Button(string text, Vector2 position, Action onClick)
        {
            _text = text;
            _onClick = onClick;
            
            // Размер кнопки пропорционален длине текста
            int width = 200;
            if (text.Length > 10)
                width = 250;
            
            _rectangle = new Rectangle((int)position.X - width/2, (int)position.Y - 25, width, 50);
        }
        
        public void Update(MouseState mouseState)
        {
            // Проверяем, наведена ли мышь на кнопку
            IsHovering = _rectangle.Contains(mouseState.X, mouseState.Y);
        }
        
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Фон кнопки
            Color backgroundColor = IsHovering ? _hoverColor : _normalColor;
            spriteBatch.Draw(GetTexture(spriteBatch.GraphicsDevice), _rectangle, backgroundColor);
            
            // Рамка кнопки
            DrawRectangleOutline(spriteBatch, _rectangle, _borderColor, IsHovering ? 2 : 1);
            
            // Текст кнопки
            Vector2 textSize = TextRenderer.MeasureString(_text);
            Vector2 textPosition = new Vector2(
                _rectangle.X + (_rectangle.Width / 2) - (textSize.X / 2),
                _rectangle.Y + (_rectangle.Height / 2) - (textSize.Y / 2)
            );
            
            TextRenderer.DrawText(spriteBatch, _text, textPosition, _textColor);
        }
        
        private void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            Texture2D pixel = GetTexture(spriteBatch.GraphicsDevice);
            
            // Верхняя линия
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // Нижняя линия
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // Левая линия
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // Правая линия
            spriteBatch.Draw(pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }
        
        public void Click()
        {
            _onClick?.Invoke();
        }
        
        private Texture2D GetTexture(GraphicsDevice graphicsDevice)
        {
            // Создаем текстуру 1x1 для рисования прямоугольников
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
    }
} 