using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SignalControl.UI
{
    public static class TextRenderer
    {
        private static Texture2D _pixel;
        private static SpriteFont _font;

        public static void Initialize(SpriteFont font)
        {
            _font = font;
        }

        // Отображаем текст используя SpriteFonts
        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1.0f)
        {
            if (_font == null)
            {
                DrawTextFallback(spriteBatch, text, position, color, scale);
                return;
            }

            try
            {
                spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            catch (ArgumentException)
            {
                // Если произошла ошибка из-за неподдерживаемых символов, используем резервный метод
                DrawTextFallback(spriteBatch, text, position, color, scale);
            }
        }

        // Резервный метод рисования текста (используется, если шрифт не загружен)
        private static void DrawTextFallback(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1.0f)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            // Начальная позиция для рисования
            float x = position.X;
            float y = position.Y;
            float charWidth = 8 * scale;
            float charHeight = 12 * scale;
            float spacing = 2 * scale;

            foreach (char c in text)
            {
                // Переход на новую строку
                if (c == '\n')
                {
                    x = position.X;
                    y += charHeight + spacing;
                    continue;
                }

                // Простое представление буквы прямоугольником
                Rectangle charRect = new Rectangle((int)x, (int)y, (int)charWidth, (int)charHeight);
                
                // Разные цвета для разных символов для различимости
                Color charColor = color;
                if (char.IsDigit(c))
                    charColor = new Color(color.R, color.G, color.B, (byte)(color.A / 2));
                
                // Рисуем фон буквы
                spriteBatch.Draw(_pixel, charRect, charColor * 0.2f);
                
                // Рисуем контур буквы
                DrawRectangleOutline(spriteBatch, charRect, charColor);

                x += charWidth + spacing;
            }
        }

        // Рисует прямоугольный контур
        private static void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness = 1)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            
            // Верхняя линия
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            // Нижняя линия
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            // Левая линия
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            // Правая линия
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }

        // Метод для отображения подсказок
        public static void DrawInstructions(SpriteBatch spriteBatch, string instructions, Vector2 position)
        {
            DrawText(spriteBatch, instructions, position, Color.White, 1.5f);
        }

        // Получить размер строки текста
        public static Vector2 MeasureString(string text, float scale = 1.0f)
        {
            if (_font != null)
            {
                return _font.MeasureString(text) * scale;
            }
            
            // Резервный расчет размера
            float charWidth = 8 * scale;
            float charHeight = 12 * scale;
            float spacing = 2 * scale;
            
            int maxLineLength = 0;
            int lines = 1;
            int lineLength = 0;
            
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    maxLineLength = Math.Max(maxLineLength, lineLength);
                    lineLength = 0;
                    lines++;
                }
                else
                {
                    lineLength++;
                }
            }
            
            maxLineLength = Math.Max(maxLineLength, lineLength);
            float width = maxLineLength * (charWidth + spacing);
            float height = lines * (charHeight + spacing);
            
            return new Vector2(width, height);
        }
    }
} 