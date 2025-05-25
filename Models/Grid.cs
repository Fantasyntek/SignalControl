using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalControl.Models
{
    public class Grid
    {
        private Node[,] _nodes;
        private Node _startNode;
        private Node _finishNode;
        private List<Node> _checkpointNodes;
        private Vector2 _position;
        private int _width;
        private int _height;
        private int _cellSize;
        
        public int Width => _width;
        public int Height => _height;
        public int CellSize => _cellSize;
        public Vector2 Position => _position;
        
        public Grid(int width, int height, Vector2 position, int cellSize)
        {
            _width = width;
            _height = height;
            _position = position;
            _cellSize = cellSize;
            _checkpointNodes = new List<Node>();
            
            // Initialize grid
            _nodes = new Node[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _nodes[x, y] = new Node(x, y, NodeType.Normal, Direction.Right);
                }
            }
        }
        
        public void SetNodeType(int x, int y, NodeType type)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                _nodes[x, y].Type = type;
                
                // Keep track of special nodes
                if (type == NodeType.Start)
                    _startNode = _nodes[x, y];
                else if (type == NodeType.Finish)
                    _finishNode = _nodes[x, y];
                else if (type == NodeType.Checkpoint)
                    _checkpointNodes.Add(_nodes[x, y]);
            }
        }
        
        public Node GetNode(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                return _nodes[x, y];
            }
            
            return null;
        }
        
        public Node GetNodeAtPosition(int screenX, int screenY)
        {
            // Check if click is within grid bounds
            if (screenX < _position.X || screenX > _position.X + _width * _cellSize ||
                screenY < _position.Y || screenY > _position.Y + _height * _cellSize)
            {
                return null;
            }
            
            // Calculate grid coordinates
            int gridX = (int)((screenX - _position.X) / _cellSize);
            int gridY = (int)((screenY - _position.Y) / _cellSize);
            
            return GetNode(gridX, gridY);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw grid cells
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _nodes[x, y].Draw(spriteBatch, new Vector2(_position.X + x * _cellSize, _position.Y + y * _cellSize), _cellSize);
                }
            }
        }
        
        public void Update(GameTime gameTime)
        {
            // Update grid cells
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _nodes[x, y].Update(gameTime);
                }
            }
        }
        
        public void ResetSignal()
        {
            foreach (Node node in _nodes)
            {
                node.IsSignalPassing = false;
                if (node.Type == NodeType.Checkpoint)
                {
                    node.IsActive = false;
                }
            }
        }
        
        public bool SimulateSignal()
        {
            if (_startNode == null || _finishNode == null)
                return false;
            
            // Reset signal passing state
            foreach (Node node in _nodes)
            {
                node.IsSignalPassing = false;
                if (node.Type == NodeType.Checkpoint)
                {
                    node.IsActive = false; // Сбрасываем активацию чекпоинтов
                }
            }
            
            // Start signal propagation from start node
            bool result = PropagateSignal(_startNode);
            
            // Проверяем, что сигнал прошел через все чекпоинты
            if (result && _checkpointNodes.Count > 0)
            {
                result = _checkpointNodes.All(checkpoint => checkpoint.IsSignalPassing);
            }
            
            return result;
        }
        
        private bool PropagateSignal(Node currentNode)
        {
            // Mark current node as having signal
            currentNode.IsSignalPassing = true;
            
            // Если это чекпоинт, активируем его
            if (currentNode.Type == NodeType.Checkpoint)
            {
                currentNode.IsActive = true;
            }
            
            // Если достигли финиша, уровень пройден
            if (currentNode == _finishNode)
            {
                return true;
            }

            // If current node is blocking and not active, signal can't pass
            if (currentNode.Type == NodeType.Blocking && !currentNode.IsActive)
            {
                return false;
            }
            
            // Get next node based on current direction
            Node nextNode = GetNextNode(currentNode);
            if (nextNode == null)
            {
                return false; // Signal hit boundary
            }
            
            // Recursive propagation
            return PropagateSignal(nextNode);
        }
        
        private Node GetNextNode(Node currentNode)
        {
            int nextX = currentNode.X;
            int nextY = currentNode.Y;

            switch (currentNode.Direction)
            {
                case Direction.Up:
                    nextY--;
                    break;
                case Direction.Right:
                    nextX++;
                    break;
                case Direction.Down:
                    nextY++;
                    break;
                case Direction.Left:
                    nextX--;
                    break;
            }

            return GetNode(nextX, nextY);
        }
    }
} 