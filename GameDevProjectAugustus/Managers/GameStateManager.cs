using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers
{
    public class GameStateManager
    {
        private static GameStateManager _instance;
        public static GameStateManager Instance => _instance ??= new GameStateManager();

        private GameState _currentState;
        private IScreen _currentScreen;
        private Game1 _game;
        private GraphicsDeviceManager _graphics;
        private SpriteFont _font;

        public GameState CurrentState => _currentState;

        private GameStateManager() { }

        public void Initialize(Game1 game, GraphicsDeviceManager graphics, SpriteFont font)
        {
            _game = game;
            _graphics = graphics;
            _font = font;
            ChangeState(GameState.Start);
        }

        public void ChangeState(GameState newState)
        {
            _currentState = newState;
            _currentScreen = null;

            switch (_currentState)
            {
                case GameState.Start:
                    _currentScreen = new StartScreen(_graphics, _font);
                    // Reset the player's health and flickering state when starting the game
                    if (_game.PlayerController is Sprite playerSprite)
                    {
                        playerSprite.ResetHealth();
                    }
                    _game.ClearEnemies();

                    break;

                case GameState.Playing:
                    _game.LoadLevel("level1");
                    Vector2 spawnPosition = _game.FindSpawnPosition(2);
                    _game.PlayerController.Initialize(spawnPosition);
                    break;

                case GameState.GameOver:
                    _currentScreen = new GameOverScreen(_graphics, _font);
                    break;

                case GameState.Finish:
                    _currentScreen = new FinishScreen(_graphics, _font);
                    break;
            }
        }

        public void Update(GameTime gameTime)
        {
            _currentScreen?.Update();
            // If Playing, update game logic
            if (_currentState == GameState.Playing)
            {
                _game.UpdateGame(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_currentState == GameState.Playing)
            {
                // The main game drawing should happen in Game1
                _game.DrawGame(spriteBatch);
            }
            else
            {
                _currentScreen?.Draw(spriteBatch);
            }
        }
    }
}
