using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SignalControl.UI
{
    public static class FontManager
    {
        private static Dictionary<int, FontSystem> _fontSystems = new Dictionary<int, FontSystem>();
        private static GraphicsDevice _graphicsDevice;

        public static void Initialize(GraphicsDevice graphicsDevice, byte[] fontData)
        {
            _graphicsDevice = graphicsDevice;
            
            // Создаем различные размеры шрифтов
            var sizes = new[] { 16, 24, 32, 48 };
            foreach (var size in sizes)
            {
                var fontSystem = new FontSystem();
                fontSystem.AddFont(fontData);
                _fontSystems[size] = fontSystem;
            }
        }

        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float size = 32)
        {
            // Находим ближайший размер шрифта
            int baseSize = 32; // Размер по умолчанию
            foreach (var fontSize in _fontSystems.Keys)
            {
                if (size <= fontSize)
                {
                    baseSize = fontSize;
                    break;
                }
            }

            var fontSystem = _fontSystems[baseSize];
            var scale = size / baseSize;
            
            // Получаем шрифт нужного размера
            var font = fontSystem.GetFont(baseSize);
            var segment = new StringSegment(text);
            
            // Рисуем тень с небольшим смещением и меньшей прозрачностью
            font.DrawText(spriteBatch, segment, position + new Vector2(1, 1), new Color(0, 0, 0, 100), 0, Vector2.Zero, Vector2.One * scale);
            
            // Рисуем основной текст
            font.DrawText(spriteBatch, segment, position, color, 0, Vector2.Zero, Vector2.One * scale);
        }

        public static Vector2 MeasureString(string text, float size = 32)
        {
            // Находим ближайший размер шрифта
            int baseSize = 32; // Размер по умолчанию
            foreach (var fontSize in _fontSystems.Keys)
            {
                if (size <= fontSize)
                {
                    baseSize = fontSize;
                    break;
                }
            }

            var fontSystem = _fontSystems[baseSize];
            var scale = size / baseSize;
            var segment = new StringSegment(text);
            
            var font = fontSystem.GetFont(baseSize);
            var bounds = font.MeasureString(text);
            return bounds * scale;
        }
    }
} 