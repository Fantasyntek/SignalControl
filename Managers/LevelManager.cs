using Microsoft.Xna.Framework;
using SignalControl.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SignalControl.Managers
{
    public class LevelManager
    {
        private List<LevelData> _levels;
        private int _currentLevelIndex;
        
        public LevelManager()
        {
            _levels = new List<LevelData>();
            _currentLevelIndex = 0;
            
            // Hard-coded levels for now
            CreateDefaultLevels();
        }
        
        private void CreateDefaultLevels()
        {
            // Level 1 - Tutorial level
            var level1 = new LevelData
            {
                GridWidth = 4,
                GridHeight = 4,
                ActionLimit = 5,
                Nodes = new List<NodeData>
                {
                    new NodeData { X = 0, Y = 0, Type = NodeType.Start, Direction = Direction.Right },
                    new NodeData { X = 3, Y = 3, Type = NodeType.Finish, Direction = Direction.Left },
                    new NodeData { X = 1, Y = 1, Type = NodeType.Normal, Direction = Direction.Right }
                },
                IsTutorial = true,
                TutorialText = "Поверните узел, чтобы направить сигнал к финишу.",
                Hint = "Нажмите на узел в позиции (1,1), чтобы повернуть его вниз."
            };
            _levels.Add(level1);
            
            // Level 2
            var level2 = new LevelData
            {
                GridWidth = 5,
                GridHeight = 5,
                ActionLimit = 10,
                Nodes = new List<NodeData>
                {
                    new NodeData { X = 0, Y = 0, Type = NodeType.Start, Direction = Direction.Right },
                    new NodeData { X = 4, Y = 4, Type = NodeType.Finish, Direction = Direction.Left },
                    new NodeData { X = 2, Y = 2, Type = NodeType.Checkpoint, Direction = Direction.Up },
                    new NodeData { X = 1, Y = 3, Type = NodeType.Blocking, Direction = Direction.Up },
                    new NodeData { X = 3, Y = 1, Type = NodeType.Blocking, Direction = Direction.Down }
                },
                Hint = "Сначала направьте сигнал к контрольной точке в центре, затем к финишу."
            };
            _levels.Add(level2);
            
            // Level 3
            var level3 = new LevelData
            {
                GridWidth = 6,
                GridHeight = 6,
                ActionLimit = 8,
                Nodes = new List<NodeData>
                {
                    new NodeData { X = 0, Y = 0, Type = NodeType.Start, Direction = Direction.Right },
                    new NodeData { X = 5, Y = 5, Type = NodeType.Finish, Direction = Direction.Left },
                    new NodeData { X = 2, Y = 2, Type = NodeType.Checkpoint, Direction = Direction.Right },
                    new NodeData { X = 4, Y = 4, Type = NodeType.Checkpoint, Direction = Direction.Up },
                    new NodeData { X = 1, Y = 3, Type = NodeType.Blocking, Direction = Direction.Up },
                    new NodeData { X = 3, Y = 1, Type = NodeType.Blocking, Direction = Direction.Down },
                    new NodeData { X = 4, Y = 2, Type = NodeType.Blocking, Direction = Direction.Left }
                },
                Hint = "Активируйте обе контрольные точки. Начните с верхней правой части сетки."
            };
            _levels.Add(level3);
        }
        
        public LevelData GetCurrentLevel()
        {
            if (_currentLevelIndex < _levels.Count)
            {
                return _levels[_currentLevelIndex];
            }
            
            return null;
        }
        
        public void LoadLevelToGrid(Grid grid)
        {
            var levelData = GetCurrentLevel();
            if (levelData == null)
                return;
                
            // Apply level data to grid
            foreach (var nodeData in levelData.Nodes)
            {
                grid.SetNodeType(nodeData.X, nodeData.Y, nodeData.Type);
                var node = grid.GetNode(nodeData.X, nodeData.Y);
                if (node != null)
                {
                    node.Type = nodeData.Type;
                    node.Direction = nodeData.Direction;
                    node.IsActive = (nodeData.Type != NodeType.Blocking);
                }
            }
        }
        
        public bool NextLevel()
        {
            if (_currentLevelIndex < _levels.Count - 1)
            {
                _currentLevelIndex++;
                return true;
            }
            
            return false;
        }
        
        public void ResetToFirstLevel()
        {
            _currentLevelIndex = 0;
        }
        
        public int GetCurrentLevelActionLimit()
        {
            var levelData = GetCurrentLevel();
            return levelData?.ActionLimit ?? 10;
        }
        
        public int GetLevelsCount()
        {
            return _levels.Count;
        }
        
        public void SetCurrentLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < _levels.Count)
            {
                _currentLevelIndex = levelIndex;
            }
        }
        
        // Future feature: Save/load levels from JSON files
        public void SaveLevelsToFile(string filePath)
        {
            string json = JsonSerializer.Serialize(_levels);
            File.WriteAllText(filePath, json);
        }
        
        public void LoadLevelsFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _levels = JsonSerializer.Deserialize<List<LevelData>>(json);
            }
        }
    }
    
    public class LevelData
    {
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int ActionLimit { get; set; }
        public List<NodeData> Nodes { get; set; }
        public bool IsTutorial { get; set; }
        public string TutorialText { get; set; }
        public string Hint { get; set; } = "Попробуйте повернуть узлы, чтобы создать путь от старта к финишу.";
    }
    
    public class NodeData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public NodeType Type { get; set; }
        public Direction Direction { get; set; }
    }
} 