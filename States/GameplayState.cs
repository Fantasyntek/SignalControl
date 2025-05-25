using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SignalControl.Managers;
using SignalControl.Models;
using SignalControl.UI;

namespace SignalControl.States
{
    public class GameplayState : GameState
    {
        private int _currentLevel;
        private Grid _grid;
        private int _remainingActions;
        private bool _isSimulating;
        private bool _hasWon;
        private bool _hasLost;
        private MouseState _previousMouseState;
        private Button _simulateButton;
        private Button _resetButton;
        private Button _backButton;
        private Button _nextLevelButton;
        private Button _hintButton;
        private LevelManager _levelManager;
        private float _gameTimer;
        private bool _showingHint;
        private float _hintTimer;
        
        // Константы для дизайна
        private readonly Color _backgroundColor = new Color((byte)30, (byte)30, (byte)60);
        private readonly Color _panelColor = new Color((byte)50, (byte)50, (byte)80); 
        private readonly Color _textColor = new Color((byte)220, (byte)220, (byte)255);
        private readonly Color _successColor = new Color((byte)0, (byte)230, (byte)118);
        private readonly Color _failColor = new Color((byte)255, (byte)82, (byte)82);
        private readonly Color _hintColor = new Color((byte)255, (byte)215, (byte)0);
        
        public GameplayState(Game game, StateManager stateManager, ContentManager content, int level) 
            : base(game, stateManager, content)
        {
            _currentLevel = level;
            _isSimulating = false;
            _hasWon = false;
            _hasLost = false;
            _levelManager = new LevelManager();
            _gameTimer = 0;
            _showingHint = false;
            _hintTimer = 0;
            
            // Устанавливаем текущий уровень
            _levelManager.SetCurrentLevel(level);
        }
        
        public override void LoadContent()
        {
            // Загружаем уровень
            LoadLevel(_currentLevel);
            
            // Создаем кнопки UI
            _simulateButton = new Button("Запустить сигнал", new Vector2(1100, 540), () => 
            {
                if (!_isSimulating && _remainingActions > 0)
                {
                    StartSimulation();
                }
            });
            
            _resetButton = new Button("Сбросить", new Vector2(1100, 600), () => 
            {
                ResetLevel();
            });
            
            _nextLevelButton = new Button("Следующий уровень", new Vector2(1100, 660), () => 
            {
                NextLevel();
            });
            
            _backButton = new Button("Меню", new Vector2(1100, 720), () => 
            {
                _stateManager.ChangeState(new MenuState(_game, _stateManager, _content));
            });
            
            _hintButton = new Button("Подсказка", new Vector2(1100, 480), () => 
            {
                ShowHint();
            });
        }
        
        private void LoadLevel(int level)
        {
            // Получаем данные уровня из менеджера
            var levelData = _levelManager.GetCurrentLevel();
            if (levelData == null)
                return;
                
            // Создаем сетку на основе размеров уровня
            _grid = new Grid(levelData.GridWidth, levelData.GridHeight, new Vector2(300, 150), 70);
            
            // Загружаем узлы в сетку
            _levelManager.LoadLevelToGrid(_grid);
            
            // Устанавливаем лимит действий
            _remainingActions = _levelManager.GetCurrentLevelActionLimit();
        }
        
        private void NextLevel()
        {
            if (_levelManager.NextLevel())
            {
                LoadLevel(_currentLevel + 1);
                _currentLevel++;
                _isSimulating = false;
                _hasWon = false;
                _hasLost = false;
                _gameTimer = 0;
            }
        }
        
        private void StartSimulation()
        {
            _isSimulating = true;
            bool result = _grid.SimulateSignal();
            
            if (result)
            {
                _hasWon = true;
            }
            else
            {
                _hasLost = true;
            }
        }
        
        private void ResetLevel()
        {
            _isSimulating = false;
            _hasWon = false;
            _hasLost = false;
            _grid.ResetSignal();
            LoadLevel(_currentLevel);
            _gameTimer = 0;
        }
        
        private void ShowHint()
        {
            _showingHint = true;
            _hintTimer = 0;
        }
        
        public override void Update(GameTime gameTime)
        {
            // Увеличиваем таймер
            _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Обновляем сетку
            _grid.Update(gameTime);
            
            // Обновляем таймер подсказки
            if (_showingHint)
            {
                _hintTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_hintTimer > 5)
                {
                    _showingHint = false;
                    _hintTimer = 0;
                }
            }
            
            MouseState currentMouseState = Mouse.GetState();
            
            // Обновляем кнопки UI
            _simulateButton.Update(currentMouseState);
            _resetButton.Update(currentMouseState);
            _backButton.Update(currentMouseState);
            _nextLevelButton.Update(currentMouseState);
            _hintButton.Update(currentMouseState);
            
            if (currentMouseState.LeftButton == ButtonState.Released && 
                _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                // Проверяем клики кнопок
                if (_simulateButton.IsHovering)
                    _simulateButton.Click();
                else if (_resetButton.IsHovering)
                    _resetButton.Click();
                else if (_backButton.IsHovering)
                    _backButton.Click();
                else if (_nextLevelButton.IsHovering && _hasWon)
                    _nextLevelButton.Click();
                else if (_hintButton.IsHovering && !_showingHint)
                    _hintButton.Click();
                // Разрешаем взаимодействие с сеткой только если не идет симуляция и игра не завершена
                else if (!_isSimulating && !_hasWon && !_hasLost)
                {
                    // Проверяем клики по сетке
                    Node clickedNode = _grid.GetNodeAtPosition(currentMouseState.X, currentMouseState.Y);
                    if (clickedNode != null && clickedNode.Type != NodeType.Start && clickedNode.Type != NodeType.Finish)
                    {
                        // Поворачиваем узел
                        clickedNode.RotateDirection();
                        
                        // Уменьшаем оставшиеся действия
                        _remainingActions--;
                        
                        // Проверяем, не закончились ли действия
                        if (_remainingActions <= 0)
                        {
                            // Автоматически запускаем симуляцию, когда кончились действия
                            StartSimulation();
                        }
                    }
                }
            }
            
            _previousMouseState = currentMouseState;
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Рисуем фон игрового поля
            DrawBackground(spriteBatch);
            
            // Рисуем боковую панель
            DrawSidePanel(spriteBatch);
            
            // Рисуем сетку
            _grid.Draw(spriteBatch);
            
            // Рисуем UI
            _hintButton.Draw(spriteBatch, null);
            _simulateButton.Draw(spriteBatch, null);
            _resetButton.Draw(spriteBatch, null);
            _backButton.Draw(spriteBatch, null);
            
            // Показываем кнопку следующего уровня только если игрок выиграл
            if (_hasWon)
            {
                _nextLevelButton.Draw(spriteBatch, null);
            }
            
            // Рисуем информацию об уровне
            DrawLevelInfo(spriteBatch);
            
            // Рисуем инструкции
            DrawInstructions(spriteBatch);
            
            // Рисуем сообщения о статусе игры
            DrawGameStatusMessage(spriteBatch);
            
            // Рисуем подсказку
            DrawHint(spriteBatch);
        }
        
        private void DrawBackground(SpriteBatch spriteBatch)
        {
            // Создаем текстуру для фона
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Рисуем фон всего экрана
            spriteBatch.Draw(
                pixel, 
                new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height), 
                _backgroundColor
            );
            
            // Рисуем фон игрового поля
            Rectangle gameArea = new Rectangle(50, 50, 800, 600);
            spriteBatch.Draw(pixel, gameArea, _panelColor);
            
            // Рисуем рамку
            int borderThickness = 2;
            DrawRectangleOutline(spriteBatch, gameArea, Color.White, borderThickness);
        }
        
        private void DrawSidePanel(SpriteBatch spriteBatch)
        {
            // Создаем текстуру для панели
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Рисуем боковую панель
            Rectangle panelRect = new Rectangle(900, 50, 330, 600);
            spriteBatch.Draw(pixel, panelRect, _panelColor);
            
            // Рисуем рамку панели
            int borderThickness = 2;
            DrawRectangleOutline(spriteBatch, panelRect, Color.White, borderThickness);
        }
        
        private void DrawRectangleOutline(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
        {
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            // Верхняя линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), 
                color
            );
            
            // Нижняя линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), 
                color
            );
            
            // Левая линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), 
                color
            );
            
            // Правая линия
            spriteBatch.Draw(
                pixel, 
                new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), 
                color
            );
        }
        
        private void DrawLevelInfo(SpriteBatch spriteBatch)
        {
            // Рисуем информацию об уровне
            TextRenderer.DrawText(
                spriteBatch,
                $"Уровень: {_currentLevel + 1}",
                new Vector2(930, 80),
                _textColor,
                1.5f
            );
            
            // Рисуем индикатор прогресса уровней
            int totalLevels = _levelManager.GetLevelsCount();
            int levelWidth = 20;
            int spacing = 5;
            int totalWidth = totalLevels * levelWidth + (totalLevels - 1) * spacing;
            int startX = 930 + (250 - totalWidth) / 2;
            int y = 120;
            
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            for (int i = 0; i < totalLevels; i++)
            {
                Color levelColor = (i == _currentLevel) ? _successColor : new Color((byte)100, (byte)100, (byte)100);
                Rectangle levelRect = new Rectangle(startX + i * (levelWidth + spacing), y, levelWidth, 10);
                spriteBatch.Draw(pixel, levelRect, levelColor);
            }
            
            TextRenderer.DrawText(
                spriteBatch,
                $"Осталось действий: {_remainingActions}",
                new Vector2(930, 150),
                _remainingActions > 0 ? _textColor : _failColor,
                1.5f
            );
            
            // Таймер
            TextRenderer.DrawText(
                spriteBatch,
                $"Время: {(int)_gameTimer} с",
                new Vector2(930, 190),
                _textColor,
                1.5f
            );
        }
        
        private void DrawInstructions(SpriteBatch spriteBatch)
        {
            if (!_isSimulating && !_hasWon && !_hasLost)
            {
                // Проверяем, является ли текущий уровень обучающим
                var levelData = _levelManager.GetCurrentLevel();
                if (levelData != null && levelData.IsTutorial && !string.IsNullOrEmpty(levelData.TutorialText))
                {
                    // Рисуем обучающее сообщение
                    Rectangle tutorialBox = new Rectangle(100, 80, 700, 100);
                    Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                    
                    // Фон сообщения
                    spriteBatch.Draw(pixel, tutorialBox, new Color((byte)0, (byte)0, (byte)0, (byte)180));
                    
                    // Рамка сообщения
                    DrawRectangleOutline(spriteBatch, tutorialBox, _textColor, 2);
                    
                    // Текст обучения
                    TextRenderer.DrawText(
                        spriteBatch,
                        levelData.TutorialText,
                        new Vector2(tutorialBox.X + 20, tutorialBox.Y + 20),
                        _textColor,
                        1.2f
                    );
                }
                else
                {
                    string instructions = 
                        "ИНСТРУКЦИЯ:\n\n" +
                        "- Нажимайте на узлы, чтобы\n" +
                        "  менять их направление\n\n" +
                        "- Проведите сигнал от старта\n" +
                        "  (зеленый) до финиша (красный)\n\n" +
                        "- Активируйте все контрольные\n" +
                        "  точки (оранжевые)\n\n" +
                        "- Действия ограничены!";
                    
                    TextRenderer.DrawInstructions(
                        spriteBatch,
                        instructions,
                        new Vector2(930, 240)
                    );
                }
            }
        }
        
        private void DrawGameStatusMessage(SpriteBatch spriteBatch)
        {
            if (_hasWon)
            {
                string winText = "УРОВЕНЬ ПРОЙДЕН!";
                
                // Анимированный победный текст
                float scale = 2.0f + (float)Math.Sin(_gameTimer * 3) * 0.2f;
                
                TextRenderer.DrawText(
                    spriteBatch,
                    winText,
                    new Vector2(400 - winText.Length * 6, 50),
                    _successColor,
                    scale
                );
            }
            else if (_hasLost)
            {
                string loseText = "НЕУДАЧА!";
                
                TextRenderer.DrawText(
                    spriteBatch,
                    loseText,
                    new Vector2(400 - loseText.Length * 6, 50),
                    _failColor,
                    2.0f
                );
                
                // Подсказка что делать дальше
                string hint = "Нажмите 'Сбросить' для повторной попытки";
                TextRenderer.DrawText(
                    spriteBatch,
                    hint,
                    new Vector2(400 - hint.Length * 4, 100),
                    _textColor,
                    1.0f
                );
            }
        }
        
        private void DrawHint(SpriteBatch spriteBatch)
        {
            if (_showingHint)
            {
                // Рисуем подсказку
                string hintText = "Подсказка: " + _levelManager.GetCurrentLevel().Hint;
                
                // Создаем фон для подсказки
                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                
                Rectangle hintBox = new Rectangle(100, 650, 700, 60);
                spriteBatch.Draw(pixel, hintBox, new Color((byte)0, (byte)0, (byte)0, (byte)180));
                DrawRectangleOutline(spriteBatch, hintBox, _hintColor, 2);
                
                TextRenderer.DrawText(
                    spriteBatch,
                    hintText,
                    new Vector2(hintBox.X + 20, hintBox.Y + 20),
                    _hintColor,
                    1.2f
                );
            }
        }
    }
}