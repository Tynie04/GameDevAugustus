using System;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GameDevProjectAugustus.Enums;

namespace GameDevProjectAugustus
{
    public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    private ILevelLoader _levelLoader;
    private Level _currentLevel;
    private Texture2D _terrainTexture;
    private Texture2D _hitboxTexture;
    private Texture2D _propsTexture;
    private Texture2D _spawnsTexture;
    private Texture2D _waterTexture;
    private Texture2D _finishTexture;
    private Texture2D _rectangleTexture;
    
    private Vector2 _camera;
    public IPlayerController _playerController;
    private ICollisionManager _collisionManager;
    private int _tileSize = 16; // Make sure this matches your CSV data

    private string _currentLevelName = "level1";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _levelLoader = new CsvLevelLoader("../../../Data");
        _camera = Vector2.Zero;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _font = Content.Load<SpriteFont>("DefaultFont"); // Load the default font

        // Initialize GameStateManager and pass it to the constructor
        GameStateManager.Instance.Initialize(this, _graphics, _font);

        // Load textures for each tileset
        _terrainTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _hitboxTexture = Content.Load<Texture2D>("hitbox");
        _propsTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _spawnsTexture = Content.Load<Texture2D>("Spawns");
        _waterTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _finishTexture = Content.Load<Texture2D>("Stairs");
        
        _font = Content.Load<SpriteFont>("DefaultFont"); // Ensure this line is present and correct

        _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        _rectangleTexture.SetData(new Color[] { new Color(255, 0, 0, 255) });

        // Load the initial level
        LoadLevel(_currentLevelName);

        // Load hero texture
        Texture2D heroTexture = Content.Load<Texture2D>("Mushroom");

        // Create animations
        Animation idleAnimation = AnimationFactory.CreateAnimationFromSingleLine(
            heroTexture, frameWidth: 32, frameHeight: 32, startFrame: 0, frameCount: 4, frameTime: 0.2f);
        Animation jumpAnimation = AnimationFactory.CreateAnimationFromSingleLine(
            heroTexture, frameWidth: 32, frameHeight: 32, startFrame: 4, frameCount: 11, frameTime: 0.1f);
        Animation attackAnimation = AnimationFactory.CreateAnimationFromSingleLine(
            heroTexture, frameWidth: 32, frameHeight: 32, startFrame: 15, frameCount: 5, frameTime: 0.1f);
        Animation hurtAnimation = AnimationFactory.CreateAnimationFromSingleLine(
            heroTexture, frameWidth: 32, frameHeight: 32, startFrame: 20, frameCount: 4, frameTime: 0.2f);
        Animation deathAnimation = AnimationFactory.CreateAnimationFromSingleLine(
            heroTexture, frameWidth: 32, frameHeight: 32, startFrame: 24, frameCount: 9, frameTime: 0.15f);

        // Create movement, physics, and collision manager
        IMovement movement = new StandardMovement(moveSpeed: 2f);
        IPhysics physics = new StandardPhysics();
        _collisionManager = new CollisionManager();

        // Create sprite and add animations
        var playerRect = new Rectangle(16, 16, 32, 32); // Default size for the player
        var player = new Sprite(movement, physics, _collisionManager, playerRect, _tileSize);

        // Initialize player at spawn point
        Vector2 spawnPosition = FindSpawnPosition(2); // Find spawn position with ID 2
        player.Initialize(spawnPosition);

        player.AddAnimation("Idle", idleAnimation);
        player.AddAnimation("Jump", jumpAnimation);
        player.AddAnimation("Attack", attackAnimation);
        player.AddAnimation("Hurt", hurtAnimation);
        player.AddAnimation("Death", deathAnimation);
        player.PlayAnimation("Idle");

        _playerController = player;

        // Set the game state to Start
        GameStateManager.Instance.ChangeState(GameState.Start);
    }

    public Vector2 FindSpawnPosition(int id)
    {
        foreach (var kvp in _currentLevel.Spawns)
        {
            if (kvp.Value == id)
            {
                return kvp.Key;
            }
        }
        return Vector2.Zero; // Default spawn position if not found
    }

    public void LoadLevel(string levelName)
    {
        _currentLevel = _levelLoader.LoadLevel(levelName);
        _currentLevelName = levelName;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // Update the game state manager
        GameStateManager.Instance.Update(gameTime);

        // Check if the player is in the Playing state
        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            KeyboardState keystate = Keyboard.GetState();
        
            // Debug: Check if key press is detected
            if (keystate.IsKeyDown(Keys.D9))
            {
                Console.WriteLine("Game Over triggered by pressing 9");
                GameStateManager.Instance.ChangeState(GameState.GameOver);
                return; // Skip further updates to immediately show the game over screen
            }

            _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);

            CheckForLevelTransition();

            UpdateCamera(); // Update camera position
        }

        base.Update(gameTime);
    }

    private void CheckForLevelTransition()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing) return;

        Rectangle playerRect = _playerController.GetRectangle();
        Vector2 playerTile = new Vector2(
            (int)(playerRect.Center.X / _tileSize),
            (int)(playerRect.Center.Y / _tileSize)
        );

        if (_currentLevel.Spawns.TryGetValue(playerTile, out int spawnId) && spawnId == 0)
        {
            if (_currentLevelName == "level2")
            {
                // Trigger the finish screen when on the last level
                GameStateManager.Instance.ChangeState(GameState.Finish);
            }
            else
            {
                LoadNextLevel();
            }
        }
    }


    private void LoadNextLevel()
    {
        if (_currentLevelName == "level1")
        {
            LoadLevel("level2");
            Vector2 spawnPosition = FindSpawnPosition(2); // Find spawn position with ID 2 in the new level
            _playerController.Initialize(spawnPosition);
        }
        // Add more levels as needed
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Let the GameStateManager handle the draw calls
        GameStateManager.Instance.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    public void DrawGame(SpriteBatch spriteBatch)
    {
        // Here, draw the main game content: levels, player, etc.
        DrawTiles(_currentLevel.Ground, _terrainTexture);
        DrawTiles(_currentLevel.Platforms, _terrainTexture);
        //DrawTiles(_currentLevel.Collisions, _hitboxTexture);
        DrawTiles(_currentLevel.Spawns, _spawnsTexture);
        DrawTiles(_currentLevel.Water, _waterTexture);
        //DrawTiles(_currentLevel.Finish, _finishTexture);
        DrawTiles(_currentLevel.Props, _propsTexture);
        
        // Draw the player character
        _playerController.Draw(spriteBatch, _camera);

        // If needed, draw the player collision rectangle for debugging
        DrawRectHollow(spriteBatch, _playerController.GetRectangle(), 2);
    }
    
    public void UpdateGame(GameTime gameTime)
    {
        // Handle game update logic during the Playing state
        KeyboardState keystate = Keyboard.GetState();
        _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);
        CheckForLevelTransition();
        UpdateCamera();
    }
    private void UpdateCamera()
    {
        Rectangle playerRect = _playerController.GetRectangle();

        _camera.X = playerRect.Center.X - (_graphics.PreferredBackBufferWidth / 2);
        _camera.Y = playerRect.Center.Y - (_graphics.PreferredBackBufferHeight / 2);

        // Clamp the camera position to the level bounds
        _camera.X = Math.Max(0, _camera.X);
        _camera.Y = Math.Max(0, _camera.Y);
        _camera.X = Math.Min(_currentLevel.Width * _tileSize - _graphics.PreferredBackBufferWidth, _camera.X);
        _camera.Y = Math.Min(_currentLevel.Height * _tileSize - _graphics.PreferredBackBufferHeight, _camera.Y);
    }

    private void DrawTiles(Dictionary<Vector2, int> tiles, Texture2D texture)
    {
        int displayTileSize = _tileSize;

        foreach (var kvp in tiles)
        {
            Vector2 position = kvp.Key;
            int tileValue = kvp.Value;

            Rectangle destinationRect = new Rectangle(
                (int)(position.X * displayTileSize - _camera.X),
                (int)(position.Y * displayTileSize - _camera.Y),
                displayTileSize,
                displayTileSize
            );

            Rectangle sourceRect = new Rectangle(
                (tileValue % 20) * displayTileSize,
                (tileValue / 20) * displayTileSize,
                displayTileSize,
                displayTileSize
            );

            _spriteBatch.Draw(texture, destinationRect, sourceRect, Color.White);
        }
    }

    public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
    {
        spriteBatch.Draw(
            _rectangleTexture,
            new Rectangle(
                rect.X - (int)_camera.X,
                rect.Y - (int)_camera.Y,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            _rectangleTexture,
            new Rectangle(
                rect.X - (int)_camera.X,
                rect.Bottom - thickness - (int)_camera.Y,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            _rectangleTexture,
            new Rectangle(
                rect.X - (int)_camera.X,
                rect.Y - (int)_camera.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
        spriteBatch.Draw(
            _rectangleTexture,
            new Rectangle(
                rect.Right - thickness - (int)_camera.X,
                rect.Y - (int)_camera.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
    }
}

}
