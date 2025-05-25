using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SignalControl.Managers;
using System;

namespace SignalControl.Models
{
    public enum NodeType
    {
        Empty,
        Start,
        Finish,
        Normal,
        Blocking,
        Checkpoint
    }
    
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
    
    public class Node
    {
        public NodeType Type { get; set; }
        public Direction Direction { get; set; }
        public bool IsActive { get; set; }
        public bool IsSignalPassing { get; set; }
        public Rectangle Bounds { get; private set; }
        
        private Vector2 _position;
        private int _size;
        private float _animationTime = 0;
        
        // Цвета для разных типов узлов
        private static readonly Color StartColor = new Color((byte)0, (byte)230, (byte)118);       // Зеленый
        private static readonly Color FinishColor = new Color((byte)255, (byte)82, (byte)82);      // Красный
        private static readonly Color NormalColor = new Color((byte)200, (byte)200, (byte)200);    // Светло-серый
        private static readonly Color BlockingColor = new Color((byte)66, (byte)66, (byte)66);     // Темно-серый
        private static readonly Color CheckpointColor = new Color((byte)255, (byte)167, (byte)38); // Оранжевый
        private static readonly Color SignalColor = new Color((byte)255, (byte)255, (byte)0);      // Желтый для сигнала
        
        public Node(Vector2 position, int size, NodeType type = NodeType.Normal)
        {
            _position = position;
            _size = size;
            Type = type;
            Direction = Direction.Right; // Направление по умолчанию
            IsActive = (type != NodeType.Blocking); // Блокирующие узлы начинаются неактивными
            IsSignalPassing = false;
            
            Bounds = new Rectangle((int)position.X, (int)position.Y, size, size);
        }
        
        public void RotateDirection()
        {
            // Поворот по часовой стрелке
            Direction = Direction switch
            {
                Direction.Up => Direction.Right,
                Direction.Right => Direction.Down,
                Direction.Down => Direction.Left,
                Direction.Left => Direction.Up,
                _ => Direction
            };
        }
        
        public void Update(GameTime gameTime)
        {
            // Обновляем анимацию
            _animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            Color nodeColor = GetNodeColor();
            
            // Фоновый прямоугольник узла
            Rectangle innerRect = new Rectangle(
                Bounds.X + 2, 
                Bounds.Y + 2, 
                Bounds.Width - 4, 
                Bounds.Height - 4
            );
            
            spriteBatch.Draw(GetTexture(spriteBatch.GraphicsDevice), innerRect, nodeColor);
            
            // Рамка узла
            int borderThickness = IsSignalPassing ? 3 : 1;
            Color borderColor = IsSignalPassing ? SignalColor : Color.Black;
            DrawRectangleOutline(spriteBatch, Bounds, borderColor, borderThickness);
            
            // Значок в зависимости от типа узла
            DrawNodeIcon(spriteBatch);
            
            // Индикатор направления
            DrawDirectionIndicator(spriteBatch);
            
            // Анимация для активного сигнала
            if (IsSignalPassing)
            {
                float pulseFactor = (float)Math.Sin(_animationTime * 5) * 0.2f + 0.8f;
                Rectangle pulseRect = new Rectangle(
                    Bounds.X + Bounds.Width/4, 
                    Bounds.Y + Bounds.Height/4, 
                    Bounds.Width/2, 
                    Bounds.Height/2
                );
                spriteBatch.Draw(GetTexture(spriteBatch.GraphicsDevice), pulseRect, SignalColor * pulseFactor);
            }
        }
        
        private void DrawNodeIcon(SpriteBatch spriteBatch)
        {
            Texture2D pixel = GetTexture(spriteBatch.GraphicsDevice);
            int iconSize = _size / 3;
            Vector2 center = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            
            switch (Type)
            {
                case NodeType.Start:
                    // Треугольник (стрелка)
                    DrawTriangle(spriteBatch, center, iconSize, StartColor);
                    break;
                case NodeType.Finish:
                    // Круг
                    DrawCircle(spriteBatch, center, iconSize/2, FinishColor);
                    break;
                case NodeType.Checkpoint:
                    // Звездочка
                    Rectangle checkRect = new Rectangle(
                        (int)center.X - iconSize/2, 
                        (int)center.Y - iconSize/2, 
                        iconSize, 
                        iconSize
                    );
                    spriteBatch.Draw(pixel, checkRect, CheckpointColor);
                    break;
                case NodeType.Blocking:
                    // Крест
                    int thickness = 3;
                    Rectangle block1 = new Rectangle(
                        (int)center.X - iconSize/2, 
                        (int)center.Y - thickness/2, 
                        iconSize, 
                        thickness
                    );
                    Rectangle block2 = new Rectangle(
                        (int)center.X - thickness/2, 
                        (int)center.Y - iconSize/2, 
                        thickness, 
                        iconSize
                    );
                    Color blockColor = IsActive ? NormalColor : BlockingColor;
                    spriteBatch.Draw(pixel, block1, blockColor);
                    spriteBatch.Draw(pixel, block2, blockColor);
                    break;
            }
        }
        
        private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, int radius, Color color)
        {
            Texture2D pixel = GetTexture(spriteBatch.GraphicsDevice);
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x*x + y*y <= radius*radius)
                    {
                        spriteBatch.Draw(pixel, 
                            new Rectangle((int)center.X + x, (int)center.Y + y, 1, 1),
                            color);
                    }
                }
            }
        }
        
        private void DrawTriangle(SpriteBatch spriteBatch, Vector2 center, int size, Color color)
        {
            Texture2D pixel = GetTexture(spriteBatch.GraphicsDevice);
            Vector2[] vertices = new Vector2[3];
            
            // Треугольник указывает в направлении стрелки
            switch (Direction)
            {
                case Direction.Right:
                    vertices[0] = center + new Vector2(size/2, 0);
                    vertices[1] = center + new Vector2(-size/2, -size/2);
                    vertices[2] = center + new Vector2(-size/2, size/2);
                    break;
                case Direction.Down:
                    vertices[0] = center + new Vector2(0, size/2);
                    vertices[1] = center + new Vector2(-size/2, -size/2);
                    vertices[2] = center + new Vector2(size/2, -size/2);
                    break;
                case Direction.Left:
                    vertices[0] = center + new Vector2(-size/2, 0);
                    vertices[1] = center + new Vector2(size/2, -size/2);
                    vertices[2] = center + new Vector2(size/2, size/2);
                    break;
                case Direction.Up:
                    vertices[0] = center + new Vector2(0, -size/2);
                    vertices[1] = center + new Vector2(-size/2, size/2);
                    vertices[2] = center + new Vector2(size/2, size/2);
                    break;
            }
            
            // Заполняем треугольник
            FillPolygon(spriteBatch, vertices, color);
        }
        
        private void FillPolygon(SpriteBatch spriteBatch, Vector2[] vertices, Color color)
        {
            if (vertices.Length < 3)
                return;
                
            Texture2D pixel = GetTexture(spriteBatch.GraphicsDevice);
            
            // Находим рамки многоугольника
            float minX = vertices[0].X;
            float minY = vertices[0].Y;
            float maxX = vertices[0].X;
            float maxY = vertices[0].Y;
            
            for (int i = 1; i < vertices.Length; i++)
            {
                minX = Math.Min(minX, vertices[i].X);
                minY = Math.Min(minY, vertices[i].Y);
                maxX = Math.Max(maxX, vertices[i].X);
                maxY = Math.Max(maxY, vertices[i].Y);
            }
            
            // Проходим по каждому пикселю и проверяем, внутри ли он многоугольника
            for (int x = (int)minX; x <= maxX; x++)
            {
                for (int y = (int)minY; y <= maxY; y++)
                {
                    if (IsPointInPolygon(vertices, new Vector2(x, y)))
                    {
                        spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), color);
                    }
                }
            }
        }
        
        private bool IsPointInPolygon(Vector2[] polygon, Vector2 point)
        {
            bool isInside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) * (point.Y - polygon[i].Y) 
                    / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    isInside = !isInside;
                }
            }
            return isInside;
        }
        
        private Color GetNodeColor()
        {
            if (IsSignalPassing)
                return Color.Lerp(GetBaseColor(), SignalColor, 0.5f);
                
            return GetBaseColor();
        }
        
        private Color GetBaseColor()
        {
            return Type switch
            {
                NodeType.Empty => NormalColor,
                NodeType.Start => StartColor,
                NodeType.Finish => FinishColor,
                NodeType.Normal => NormalColor,
                NodeType.Blocking => IsActive ? Color.Lerp(BlockingColor, NormalColor, 0.7f) : BlockingColor,
                NodeType.Checkpoint => IsActive ? CheckpointColor : Color.Lerp(CheckpointColor, BlockingColor, 0.5f),
                _ => NormalColor
            };
        }
        
        private void DrawDirectionIndicator(SpriteBatch spriteBatch)
        {
            // Используем текстуру стрелки из TextureManager
            Vector2 center = new Vector2(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
            
            // Определяем позицию для отрисовки стрелки (центрируем в ячейке)
            int arrowSize = _size / 2;
            Vector2 arrowPosition = new Vector2(
                center.X - arrowSize / 2,
                center.Y - arrowSize / 2
            );
            
            // Отрисовываем стрелку с помощью TextureManager
            Managers.TextureManager.DrawArrow(spriteBatch, Direction, arrowPosition, 0.5f);
        }
        
        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, int thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            
            spriteBatch.Draw(
                GetTexture(spriteBatch.GraphicsDevice),
                new Rectangle((int)start.X, (int)start.Y, (int)edge.Length(), thickness),
                null,
                color,
                angle,
                new Vector2(0, 0),
                SpriteEffects.None,
                0
            );
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
        
        private Texture2D GetTexture(GraphicsDevice graphicsDevice)
        {
            // Создаем текстуру 1x1 для рисования примитивов
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData(new[] { Color.White });
            return texture;
        }
        
        public void ResetSignal()
        {
            IsSignalPassing = false;
            
            // Сбрасываем активное состояние только для блокирующих узлов
            if (Type == NodeType.Blocking)
                IsActive = false;
                
            // Контрольные точки должны оставаться активными, если они были активированы
        }
        
        public void ToggleActiveState()
        {
            if (Type == NodeType.Blocking)
            {
                IsActive = !IsActive;
            }
        }
    }
} 