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
            
            // Измеряем размер текста для определения минимальной ширины кнопки
            Vector2 textSize = TextRenderer.MeasureString(text);
            
            // Фиксированный размер для всех кнопок
            int width = 200; // Фиксированная ширина для всех кнопок
            int height = 60; // Фиксированная высота для всех кнопок
            
            _rectangle = new Rectangle((int)position.X - width/2, (int)position.Y - height/2, width, height);
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
            
            // Рамка кнопки с увеличенной толщиной
            DrawRectangleOutline(spriteBatch, _rectangle, _borderColor, IsHovering ? 3 : 2);
            
            // Измеряем текст для точного центрирования
            Vector2 textSize = TextRenderer.MeasureString(_text);
            
            // Вычисляем масштаб текста, если он не помещается
            float scale = 1.2f; // Увеличиваем базовый размер текста
            if (textSize.X * scale > _rectangle.Width - 40 || textSize.Y * scale > _rectangle.Height - 20)
            {
                scale = Math.Min(
                    (_rectangle.Width - 40) / textSize.X,
                    (_rectangle.Height - 20) / textSize.Y
                );
            }
            
            // Пересчитываем размер текста с учетом масштаба
            Vector2 scaledTextSize = textSize * scale;
            
            // Вычисляем позицию для центрирования
            Vector2 textPosition = new Vector2(
                _rectangle.X + (_rectangle.Width - scaledTextSize.X) / 2,
                _rectangle.Y + (_rectangle.Height - scaledTextSize.Y) / 2
            );
            
            // Рисуем текст
            TextRenderer.DrawText(spriteBatch, _text, textPosition, _textColor, scale);
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