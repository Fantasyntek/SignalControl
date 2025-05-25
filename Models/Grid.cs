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
        private Vector2 _position;
        private int _cellSize;
        private int _width;
        private int _height;
        private Node _startNode;
        private Node _finishNode;
        private List<Node> _checkpointNodes;
        
        public Grid(int width, int height, Vector2 position, int cellSize)
        {
            _width = width;
            _height = height;
            _position = position;
            _cellSize = cellSize;
            _checkpointNodes = new List<Node>();
            
            InitializeGrid();
        }
        
        private void InitializeGrid()
        {
            _nodes = new Node[_width, _height];
            
            // Create nodes
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Vector2 nodePosition = new Vector2(
                        _position.X + x * _cellSize,
                        _position.Y + y * _cellSize
                    );
                    
                    _nodes[x, y] = new Node(nodePosition, _cellSize);
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
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _nodes[x, y].Draw(spriteBatch);
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
            }
            
            // Start signal propagation from start node
            return PropagateSignal(_startNode);
        }
        
        private bool PropagateSignal(Node currentNode)
        {
            // Mark current node as having signal
            currentNode.IsSignalPassing = true;
            
            // Activate checkpoint if we pass through it
            if (currentNode.Type == NodeType.Checkpoint)
            {
                currentNode.IsActive = true;
            }

            // If current node is blocking and not active, signal can't pass
            if (currentNode.Type == NodeType.Blocking && !currentNode.IsActive)
            {
                return false;
            }
            
            // If we reached finish node and all checkpoints are active, it's a win
            if (currentNode == _finishNode)
            {
                return _checkpointNodes.All(checkpoint => checkpoint.IsActive);
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
            int x = (int)((currentNode.Bounds.X - _position.X) / _cellSize);
            int y = (int)((currentNode.Bounds.Y - _position.Y) / _cellSize);
            
            // Get coordinates of next node based on direction
            switch (currentNode.Direction)
            {
                case Direction.Up:
                    y--;
                    break;
                case Direction.Right:
                    x++;
                    break;
                case Direction.Down:
                    y++;
                    break;
                case Direction.Left:
                    x--;
                    break;
            }
            
            // Check if next position is valid
            if (x >= 0 && x < _width && y >= 0 && y < _height)
            {
                return _nodes[x, y];
            }
            
            return null;
        }
        
        public void ResetSignal()
        {
            foreach (var node in _nodes)
            {
                node.ResetSignal();
            }
        }

        public void Update(GameTime gameTime)
        {
            // Обновляем все узлы
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _nodes[x, y].Update(gameTime);
                }
            }
        }
    }
} 