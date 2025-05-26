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

        public static void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1.0f)
        {
            if (_font != null)
            {
                // Draw shadow
                spriteBatch.DrawString(_font, text, position + new Vector2(2, 2), new Color(0, 0, 0, 100), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
                // Draw main text
                spriteBatch.DrawString(_font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            else
            {
                DrawTextFallback(spriteBatch, text, position, color, scale);
            }
        }

        private static void DrawTextFallback(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale = 1.0f)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            float x = position.X;
            float y = position.Y;
            float charWidth = 8 * scale;
            float charHeight = 12 * scale;
            float spacing = 2 * scale;

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    x = position.X;
                    y += charHeight + spacing;
                    continue;
                }

                Rectangle charRect = new Rectangle((int)x, (int)y, (int)charWidth, (int)charHeight);
                
                Color charColor = color;
                if (char.IsDigit(c))
                    charColor = new Color(color.R, color.G, color.B, (byte)(color.A / 2));
                
                spriteBatch.Draw(_pixel, charRect, charColor * 0.2f);
                DrawRectangleOutline(spriteBatch, charRect, charColor);

                x += charWidth + spacing;
            }
        }

        private static void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness = 1)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }
            
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
            spriteBatch.Draw(_pixel, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
        }

        public static void DrawInstructions(SpriteBatch spriteBatch, string instructions, Vector2 position)
        {
            DrawText(spriteBatch, instructions, position, Color.White, 1.5f);
        }

        public static Vector2 MeasureString(string text, float scale = 1.0f)
        {
            if (_font != null)
            {
                return _font.MeasureString(text) * scale;
            }
            
            // Fallback measurement
            float charWidth = 8;
            float charHeight = 12;
            float spacing = 2;
            
            float maxWidth = 0;
            float currentWidth = 0;
            float height = charHeight;
            int lines = 1;
            
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    maxWidth = Math.Max(maxWidth, currentWidth);
                    currentWidth = 0;
                    lines++;
                    continue;
                }
                
                currentWidth += charWidth + spacing;
            }
            
            maxWidth = Math.Max(maxWidth, currentWidth);
            return new Vector2(maxWidth, height * lines + spacing * (lines - 1)) * scale;
        }
    }
} 