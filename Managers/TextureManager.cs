using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SignalControl.Models;
using System;

namespace SignalControl.Managers
{
    public static class TextureManager
    {
        private static Texture2D _pixelTexture;
        private static Texture2D _arrowTexture;
        private static GraphicsDevice _graphicsDevice;
        private static ContentManager _content;

        public static void Initialize(GraphicsDevice graphicsDevice, ContentManager content)
        {
            _graphicsDevice = graphicsDevice;
            _content = content;

            // Создаем текстуру 1x1 для рисования примитивов
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // Загружаем текстуру стрелки из файла
            _arrowTexture = _content.Load<Texture2D>("arrow");
        }
        
        public static void DrawArrow(SpriteBatch spriteBatch, Direction direction, Vector2 position, float scale = 1.0f)
        {
            if (_arrowTexture == null)
                return;
            
            // Размер стрелки (используем размер текстуры)
            int arrowSize = (int)(_arrowTexture.Width * scale);
            
            // Вычисляем позицию для центрирования стрелки в ячейке
            int cellSize = (int)(position.Y + arrowSize * 2) - (int)position.Y;
            
            // Вычисляем смещение для центрирования
            int offsetX = (cellSize - arrowSize) / 2;
            int offsetY = (cellSize - arrowSize) / 2;
            
            // Определяем угол поворота в зависимости от направления
            float rotation = direction switch
            {
                Direction.Up => MathHelper.Pi * 1.5f,    // 270 градусов
                Direction.Right => 0f,                   // 0 градусов (исходное направление)
                Direction.Down => MathHelper.Pi * 0.5f,  // 90 градусов
                Direction.Left => MathHelper.Pi,         // 180 градусов
                _ => 0f
            };

            // Рисуем стрелку с поворотом
            spriteBatch.Draw(
                _arrowTexture,
                new Rectangle(
                    (int)position.X + offsetX, 
                    (int)position.Y + offsetY, 
                    arrowSize, 
                    arrowSize
                ),
                null,
                Color.White,
                rotation,
                new Vector2(_arrowTexture.Width / 2, _arrowTexture.Height / 2),
                SpriteEffects.None,
                0
            );
        }
        
        private static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0
            );
        }

        public static Texture2D GetPixelTexture()
        {
            return _pixelTexture;
        }

        public static Texture2D GetArrowTexture()
        {
            return _arrowTexture;
        }
    }
} 