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

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            // Создаем текстуру 1x1 для рисования примитивов
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        
        // Отрисовка буквы для конкретного направления вместо стрелки
        public static void DrawArrow(SpriteBatch spriteBatch, Direction direction, Vector2 position, float scale = 1.0f)
        {
            if (_pixelTexture == null)
                return;
            
            // Размер буквы
            int letterSize = (int)(18 * scale);
            Vector2 center = new Vector2(position.X + letterSize, position.Y + letterSize);
            Color letterColor = Color.Black;
            int thickness = 2;
            
            // Рисуем букву в зависимости от направления
            switch (direction)
            {
                case Direction.Up:
                    // Буква U
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, letterSize/2), 
                             center - new Vector2(letterSize/2, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, -letterSize/2), 
                             center - new Vector2(-letterSize/2, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(-letterSize/2, -letterSize/2), 
                             center - new Vector2(-letterSize/2, letterSize/2), letterColor, thickness);
                    break;
                case Direction.Right:
                    // Буква R
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, letterSize/2), 
                             center - new Vector2(letterSize/2, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, -letterSize/2), 
                             center - new Vector2(-letterSize/4, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(-letterSize/4, -letterSize/2), 
                             center - new Vector2(-letterSize/4, 0), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, 0), 
                             center - new Vector2(-letterSize/4, 0), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(-letterSize/4, 0), 
                             center - new Vector2(-letterSize/2, letterSize/2), letterColor, thickness);
                    break;
                case Direction.Down:
                    // Буква D
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, letterSize/2), 
                             center - new Vector2(letterSize/2, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, -letterSize/2), 
                             center - new Vector2(0, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(0, -letterSize/2), 
                             center - new Vector2(-letterSize/2, 0), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(-letterSize/2, 0), 
                             center - new Vector2(0, letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(0, letterSize/2), 
                             center - new Vector2(letterSize/2, letterSize/2), letterColor, thickness);
                    break;
                case Direction.Left:
                    // Буква L
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, letterSize/2), 
                             center - new Vector2(letterSize/2, -letterSize/2), letterColor, thickness);
                    DrawLine(spriteBatch, center - new Vector2(letterSize/2, letterSize/2), 
                             center - new Vector2(-letterSize/2, letterSize/2), letterColor, thickness);
                    break;
            }
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
    }
} 