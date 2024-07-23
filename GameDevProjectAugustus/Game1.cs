using System;
using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private ILevelLoader _levelLoader;
    private Level _currentLevel;
    private Texture2D _terrainTexture;
    private Texture2D _hitboxTexture;
    private Texture2D _propsTexture;
    private Texture2D _spawnsTexture;
    private Texture2D _waterTexture;
    private Texture2D _finishTexture;

    private Vector2 _camera;
    private IPlayerController _playerController;
    private ICollisionManager _collisionManager;
    private int _tileSize = 16; // Make sure this matches your CSV data
    private Texture2D _rectangleTexture;

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

        // Load textures for each tileset
        _terrainTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _hitboxTexture = Content.Load<Texture2D>("hitbox");
        _propsTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _spawnsTexture = Content.Load<Texture2D>("Spawns");
        _waterTexture = Content.Load<Texture2D>("Terrain_and_Props");
        _finishTexture = Content.Load<Texture2D>("Stairs"); // Add this line

        _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        _rectangleTexture.SetData(new Color[] { new Color(255, 0, 0, 255) });

        // Load the level
        _currentLevel = _levelLoader.LoadLevel("level1");

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
        IMovement movement = new StandardMovement(moveSpeed: 3f);
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
    }

    private Vector2 FindSpawnPosition(int id)
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
    
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState keystate = Keyboard.GetState();
        _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);

        UpdateCamera(); // Update camera position

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw layers in the specified order
        DrawTiles(_currentLevel.Spawns, _spawnsTexture);
        DrawTiles(_currentLevel.Collisions, _hitboxTexture);
        DrawTiles(_currentLevel.Ground, _terrainTexture);
        DrawTiles(_currentLevel.Water, _waterTexture);
        DrawTiles(_currentLevel.Platforms, _terrainTexture);
        DrawTiles(_currentLevel.Finish, _finishTexture);
        DrawTiles(_currentLevel.Props, _propsTexture);

        _playerController.Draw(_spriteBatch, _camera); // Pass camera position
        DrawRectHollow(_spriteBatch, _playerController.GetRectangle(), 4);

        _spriteBatch.End();

        base.Draw(gameTime);
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
