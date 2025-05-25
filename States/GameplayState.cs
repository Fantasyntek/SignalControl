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
            
            // Определяем размеры и позиции для UI
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            float gridWidth = _grid.Width * _grid.CellSize;
            float gridHeight = _grid.Height * _grid.CellSize;
            
            // Фиксированные размеры для боковой панели
            float panelWidth = 280;
            float panelX = screenWidth - panelWidth - 20; // 20px отступ справа
            
            // Вычисляем размеры боковой панели
            float panelHeight = screenHeight - 50;
            float buttonAreaStart = 250; // Увеличили отступ сверху для кнопок
            float buttonSpacing = 80; // Расстояние между кнопками
            
            // Центрируем кнопки по ширине панели
            float buttonX = panelX + panelWidth/2;
            
            // Создаем кнопки UI с правильным позиционированием внутри панели
            _hintButton = new Button("Подсказка", new Vector2(buttonX, buttonAreaStart), () => 
            {
                ShowHint();
            });

            _simulateButton = new Button("Запустить сигнал", new Vector2(buttonX, buttonAreaStart + buttonSpacing), () => 
            {
                if (!_isSimulating && _remainingActions > 0)
                {
                    StartSimulation();
                }
            });
            
            _resetButton = new Button("Сбросить", new Vector2(buttonX, buttonAreaStart + buttonSpacing * 2), () => 
            {
                ResetLevel();
            });
            
            _nextLevelButton = new Button("Следующий уровень", new Vector2(buttonX, buttonAreaStart + buttonSpacing * 3), () => 
            {
                NextLevel();
            });
            
            _backButton = new Button("Меню", new Vector2(buttonX, buttonAreaStart + buttonSpacing * 4), () => 
            {
                _stateManager.ChangeState(new MenuState(_game, _stateManager, _content));
            });
        }
        
        private void LoadLevel(int level)
        {
            // Получаем данные уровня из менеджера
            var levelData = _levelManager.GetCurrentLevel();
            if (levelData == null)
                return;
                
            // Определяем позицию сетки для центрирования
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            float gridWidth = levelData.GridWidth * 70; // 70 - размер ячейки
            float gridHeight = levelData.GridHeight * 70;
            
            // Централизуем сетку по горизонтали, оставляя место для панели справа
            float gridX = (screenWidth - 300 - gridWidth) / 2;
            float gridY = (screenHeight - gridHeight) / 2 - 50; // Немного выше центра
            
            // Создаем сетку на основе размеров уровня
            _grid = new Grid(levelData.GridWidth, levelData.GridHeight, new Vector2(gridX, gridY), 70);
            
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
            if (_isSimulating) // Если симуляция уже запущена, не запускаем снова
                return;
                
            _isSimulating = true;
            bool result = _grid.SimulateSignal();
            
            if (result)
            {
                _hasWon = true;
            }
            else
            {
                _hasLost = true;
                // Автоматически сбрасываем уровень через небольшую задержку
                _gameTimer = 0;
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
            
            // Если проиграли и прошло 5 секунд, сбрасываем уровень
            if (_hasLost && _gameTimer >= 5)
            {
                ResetLevel();
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
            
            // Определяем размеры и позицию игровой области
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            float gridWidth = _grid.Width * _grid.CellSize;
            float gridHeight = _grid.Height * _grid.CellSize;
            float gridCenterX = (_grid.Position.X + gridWidth / 2);
            
            // Рисуем фон игрового поля с адаптацией под размер сетки
            int gameAreaPadding = 50; // Отступ от сетки до края игровой области
            Rectangle gameArea = new Rectangle(
                (int)Math.Max(_grid.Position.X - gameAreaPadding, 50),
                (int)Math.Max(_grid.Position.Y - gameAreaPadding, 50),
                (int)(gridWidth + gameAreaPadding * 2),
                (int)(gridHeight + gameAreaPadding * 2)
            );
            
            spriteBatch.Draw(pixel, gameArea, _panelColor);
            
            // Рисуем рамку
            int borderThickness = 2;
            DrawRectangleOutline(spriteBatch, gameArea, Color.White, borderThickness);
        }
        
        private void DrawSidePanel(SpriteBatch spriteBatch)
        {
            // Используем те же значения, что и при создании кнопок
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float screenHeight = _game.GraphicsDevice.Viewport.Height;
            float panelWidth = 280;
            float panelX = screenWidth - panelWidth - 20;
            
            // Боковая панель
            Rectangle panelRect = new Rectangle(
                (int)panelX,
                25, // Отступ сверху
                (int)panelWidth,
                (int)(screenHeight - 50) // Отступ снизу
            );
            
            // Рисуем фон панели
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, panelRect, _panelColor);
            
            // Рисуем рамку панели
            DrawRectangleOutline(spriteBatch, panelRect, Color.White, 2);
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
            // Используем те же значения для позиционирования
            float screenWidth = _game.GraphicsDevice.Viewport.Width;
            float panelWidth = 280;
            float panelX = screenWidth - panelWidth - 20;
            float textX = panelX + 20; // Отступ текста от края панели
            
            // Рисуем информацию об уровне с увеличенным отступом сверху
            TextRenderer.DrawText(
                spriteBatch,
                $"Уровень: {_currentLevel + 1}",
                new Vector2(textX, 40), // Уменьшили отступ сверху
                _textColor,
                1.0f
            );
            
            // Рисуем индикатор прогресса уровней
            int totalLevels = _levelManager.GetLevelsCount();
            int levelWidth = 20;
            int spacing = 5;
            int totalWidth = totalLevels * levelWidth + (totalLevels - 1) * spacing;
            int startX = (int)panelX + ((int)panelWidth - totalWidth) / 2;
            int y = 80; // Уменьшили отступ для индикатора прогресса
            
            Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            
            for (int i = 0; i < totalLevels; i++)
            {
                Color levelColor = (i == _currentLevel) ? _successColor : new Color((byte)100, (byte)100, (byte)100);
                Rectangle levelRect = new Rectangle(startX + i * (levelWidth + spacing), y, levelWidth, 10);
                spriteBatch.Draw(pixel, levelRect, levelColor);
            }
            
            // Рисуем оставшиеся действия
            TextRenderer.DrawText(
                spriteBatch,
                $"Осталось действий: {_remainingActions}",
                new Vector2(textX, 130), // Уменьшили отступ
                _remainingActions > 0 ? _textColor : _failColor,
                1.0f
            );
            
            // Таймер
            TextRenderer.DrawText(
                spriteBatch,
                $"Время: {(int)_gameTimer} с",
                new Vector2(textX, 170), // Уменьшили отступ
                _textColor,
                1.0f
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
                    float screenWidth = _game.GraphicsDevice.Viewport.Width;
                    float screenHeight = _game.GraphicsDevice.Viewport.Height;
                    float panelWidth = 280;
                    float panelX = screenWidth - panelWidth - 20;
                    
                    // Измеряем текст для определения размеров
                    Vector2 textSize = TextRenderer.MeasureString(levelData.TutorialText, 1.3f);
                    
                    // Добавляем отступы для текста
                    float textPadding = 40; // Отступ от текста до края фона
                    float frameWidth = textSize.X + textPadding * 2;
                    float frameHeight = textSize.Y + textPadding;
                    
                    // Позиционируем блок по центру под игровым полем
                    float centerX = (_grid.Position.X + _grid.Width * _grid.CellSize / 2);
                    float centerY = _grid.Position.Y + _grid.CellSize * _grid.Height + 60;
                    
                    // Создаем фоновый прямоугольник с отступами
                    Rectangle backgroundBox = new Rectangle(
                        (int)(centerX - frameWidth / 2),
                        (int)centerY,
                        (int)frameWidth,
                        (int)frameHeight
                    );
                    
                    Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                    
                    // Рисуем внешнюю рамку с эффектом свечения
                    Rectangle glowBox = new Rectangle(
                        backgroundBox.X - 2,
                        backgroundBox.Y - 2,
                        backgroundBox.Width + 4,
                        backgroundBox.Height + 4
                    );
                    spriteBatch.Draw(pixel, glowBox, new Color((byte)100, (byte)100, (byte)200, (byte)30));
                    
                    // Фон сообщения
                    spriteBatch.Draw(pixel, backgroundBox, new Color((byte)20, (byte)20, (byte)40, (byte)230));
                    
                    // Рамка сообщения
                    DrawRectangleOutline(spriteBatch, backgroundBox, new Color((byte)100, (byte)100, (byte)200), 2);
                    
                    // Вычисляем позицию для идеального центрирования текста
                    Vector2 textPosition = new Vector2(
                        backgroundBox.X + (backgroundBox.Width - textSize.X) / 2,
                        backgroundBox.Y + (backgroundBox.Height - textSize.Y) / 2
                    );
                    
                    // Рисуем тень текста
                    TextRenderer.DrawText(
                        spriteBatch,
                        levelData.TutorialText,
                        textPosition + new Vector2(2, 2),
                        new Color((byte)0, (byte)0, (byte)0, (byte)128),
                        1.3f
                    );
                    
                    // Рисуем основной текст
                    TextRenderer.DrawText(
                        spriteBatch,
                        levelData.TutorialText,
                        textPosition,
                        _textColor,
                        1.3f
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
                float centerX = _grid.Position.X + _grid.Width * _grid.CellSize / 2;
                
                Vector2 textSize = TextRenderer.MeasureString(winText, scale);
                
                TextRenderer.DrawText(
                    spriteBatch,
                    winText,
                    new Vector2(centerX - textSize.X / 2, 20),
                    _successColor,
                    scale
                );
            }
            else if (_hasLost)
            {
                string loseText = "НЕУДАЧА";
                float scale = 2.0f + (float)Math.Sin(_gameTimer * 3) * 0.1f; // Добавляем анимацию пульсации
                float centerX = _grid.Position.X + _grid.Width * _grid.CellSize / 2;
                
                Vector2 textSize = TextRenderer.MeasureString(loseText, scale);
                float textY = _grid.Position.Y - 80; // Размещаем текст над игровым полем
                
                // Рисуем тень для текста
                TextRenderer.DrawText(
                    spriteBatch,
                    loseText,
                    new Vector2(centerX - textSize.X / 2 + 2, textY + 2),
                    new Color(0, 0, 0, 150),
                    scale
                );
                
                // Рисуем основной текст
                TextRenderer.DrawText(
                    spriteBatch,
                    loseText,
                    new Vector2(centerX - textSize.X / 2, textY),
                    _failColor,
                    scale
                );
                
                // Подсказка что делать дальше
                string hint = "Нажмите 'Сбросить' для повторной попытки";
                Vector2 hintSize = TextRenderer.MeasureString(hint, 1.0f);
                
                // Рисуем тень для подсказки
                TextRenderer.DrawText(
                    spriteBatch,
                    hint,
                    new Vector2(centerX - hintSize.X / 2 + 1, textY + 50 + 1),
                    new Color(0, 0, 0, 150),
                    1.0f
                );
                
                // Рисуем текст подсказки
                TextRenderer.DrawText(
                    spriteBatch,
                    hint,
                    new Vector2(centerX - hintSize.X / 2, textY + 50),
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
                
                // Вычисляем размеры и позицию подсказки для центрирования
                Vector2 hintSize = TextRenderer.MeasureString(hintText, 1.2f);
                float centerX = _grid.Position.X + _grid.Width * _grid.CellSize / 2;
                float boxWidth = Math.Max(hintSize.X + 40, 700);
                float boxHeight = Math.Max(hintSize.Y + 40, 60);
                
                // Создаем фон для подсказки
                Texture2D pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
                
                Rectangle hintBox = new Rectangle(
                    (int)(centerX - boxWidth / 2), 
                    (int)(_grid.Position.Y + _grid.Height * _grid.CellSize + 30),
                    (int)boxWidth, 
                    (int)boxHeight
                );
                
                spriteBatch.Draw(pixel, hintBox, new Color((byte)0, (byte)0, (byte)0, (byte)180));
                DrawRectangleOutline(spriteBatch, hintBox, _hintColor, 2);
                
                TextRenderer.DrawText(
                    spriteBatch,
                    hintText,
                    new Vector2(hintBox.X + 20, hintBox.Y + (hintBox.Height - hintSize.Y) / 2),
                    _hintColor,
                    1.2f
                );
            }
        }
    }
}