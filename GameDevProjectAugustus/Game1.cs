using System;
using System.Collections.Generic;
using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;

    private readonly ILevelLoader _levelLoader;
    private Level _currentLevel;
    private Texture2D _terrainTexture;
    private Texture2D _hitboxTexture;
    private Texture2D _propsTexture;
    private Texture2D _spawnsTexture;
    private Texture2D _waterTexture;
    private Texture2D _finishTexture;
    private Texture2D _rectangleTexture;
    private Texture2D _fullHeartTexture;
    private Texture2D _emptyHeartTexture;

    private Vector2 _camera;
    public IPlayerController _playerController;
    private ICollisionManager _collisionManager;
    private IPhysics _physics;
    private readonly int _tileSize = 16; // Make sure this matches your CSV data

    private readonly float _damageCooldown = 0.5f; // seconds
    private float _damageCooldownTimer;

    private string _currentLevelName = "level1";

    private List<IEnemy> _enemies;
    

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _levelLoader = new CsvLevelLoader("../../../Data");
        _camera = Vector2.Zero;
        
        _enemies = new List<IEnemy>();
    }

    protected override void LoadContent()
{
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    _font = Content.Load<SpriteFont>("DefaultFont");

    var walkerSpriteSheet = Content.Load<Texture2D>("walker");
    if (walkerSpriteSheet == null)
    {
        throw new Exception("Failed to load walker sprite sheet.");
    }

    // Initialize GameStateManager
    GameStateManager.Instance.Initialize(this, _graphics, _font);

    // Initialize collision manager and physics
    _collisionManager = new CollisionManager();
    _physics = new StandardPhysics();

    LoadLevel(_currentLevelName);
    if (_currentLevel == null)
    {
        throw new InvalidOperationException("Level has not been loaded.");
    }

    // Load other textures
    _hitboxTexture = Content.Load<Texture2D>("hitboxes");
    _terrainTexture = Content.Load<Texture2D>("Terrain_and_Props");
    _propsTexture = Content.Load<Texture2D>("Terrain_and_Props");
    _spawnsTexture = Content.Load<Texture2D>("Spawns");
    _waterTexture = Content.Load<Texture2D>("Terrain_and_Props");
    _finishTexture = Content.Load<Texture2D>("Stairs");

    _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
    _rectangleTexture.SetData(new[] { Color.Red });

    // Load heart textures
    _fullHeartTexture = Content.Load<Texture2D>("Heart Stage 1");
    _emptyHeartTexture = Content.Load<Texture2D>("Heart Stage 5");

    // Create player and animations
    var heroTexture = Content.Load<Texture2D>("Mushroom");

    var idleAnimation = AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 0, 4, 0.2);
    var jumpAnimation = AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 4, 11, 0.1);
    var attackAnimation = AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 15, 5, 0.1);
    var hurtAnimation = AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 20, 4, 0.25);
    var deathAnimation = AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 24, 9, 0.15);

    // Create player
    var playerRect = new Rectangle(16, 16, 32, 32);
    IHealth playerHealth = new Health(5);
    var player = new Sprite(new StandardMovement(2f), new StandardPhysics(), new CollisionManager(), playerHealth,
        playerRect, _tileSize);

    var spawnPosition = FindSpawnPosition(2); // Find spawn position with ID 2
    player.Initialize(spawnPosition);

    player.AddAnimation("Idle", idleAnimation);
    player.AddAnimation("Jump", jumpAnimation);
    player.AddAnimation("Attack", attackAnimation);
    player.AddAnimation("Hurt", hurtAnimation);
    player.AddAnimation("Death", deathAnimation);
    player.PlayAnimation("Idle");

    _playerController = player;
    player.OnDeath += Player_OnDeath;

    // Set the game state to Start
    GameStateManager.Instance.ChangeState(GameState.Start);
}


    private void Player_OnDeath(object sender, EventArgs e)
    {
        GameStateManager.Instance.ChangeState(GameState.GameOver);
    }

    public Vector2 FindSpawnPosition(int id)
    {
        foreach (var kvp in _currentLevel.Spawns)
            if (kvp.Value == id)
                return kvp.Key;
        return Vector2.Zero;
    }

    public void LoadLevel(string levelName)
    {
        Console.WriteLine($"Loading level: {levelName}");

        ClearEnemies();
    
        _currentLevel = _levelLoader.LoadLevel(levelName);
        if (_currentLevel == null)
        {
            throw new InvalidOperationException("Failed to load the level.");
        }

        _currentLevelName = levelName;

        // Spawn enemies
        SpawnWalkerEnemies();
        SpawnHiderEnemies(); // Add this line to spawn HiderEnemies
    }



    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update the game state manager
        GameStateManager.Instance.Update(gameTime);

        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            var keystate = Keyboard.GetState();

            // Update damage cooldown timer
            _damageCooldownTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keystate.IsKeyDown(Keys.P) && _damageCooldownTimer >= _damageCooldown)
            {
                _damageCooldownTimer = 0f;

                if (_playerController.IsAlive) _playerController.TakeDamage(1);
            }

            _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);

            // Update enemies
            foreach (var enemy in _enemies) enemy.Update(gameTime);

            CheckForLevelTransition();
            UpdateCamera();
        }

        base.Update(gameTime);
    }

    private void CheckForLevelTransition()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing) return;

        var playerRect = _playerController.GetRectangle();
        var playerTile = new Vector2(
            playerRect.Center.X / _tileSize,
            playerRect.Center.Y / _tileSize
        );

        if (_currentLevel.Spawns.TryGetValue(playerTile, out var spawnId) && spawnId == 0)
        {
            if (_currentLevelName == "level2")
                GameStateManager.Instance.ChangeState(GameState.Finish);
            else
                LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        if (_currentLevelName == "level1")
        {
            LoadLevel("level2");
            var spawnPosition = FindSpawnPosition(2); // Find spawn position with ID 2 in the new level
            _playerController.Initialize(spawnPosition);
        }
        // Add more levels as needed
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        GameStateManager.Instance.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void DrawGame(SpriteBatch spriteBatch)
    {
        // Draw level tiles
        DrawTiles(_currentLevel.Collisions, _hitboxTexture);
        DrawTiles(_currentLevel.Water, _waterTexture);
        DrawTiles(_currentLevel.Props, _propsTexture);
        DrawTiles(_currentLevel.Ground, _terrainTexture);
        DrawTiles(_currentLevel.Platforms, _terrainTexture);
        DrawTiles(_currentLevel.Spawns, _spawnsTexture);

        // Draw enemies
        foreach (var enemy in _enemies) enemy.Draw(spriteBatch, _camera);

        // Draw player
        _playerController.Draw(spriteBatch, _camera);

        // Draw health hearts
        DrawHealth(spriteBatch);

        // Draw player collision rectangle for debugging
        DrawRectHollow(spriteBatch, _playerController.GetRectangle(), 2);
    }

    private void DrawHealth(SpriteBatch spriteBatch)
    {
        var scale = 0.5f;
        var heartSize = (int)(_fullHeartTexture.Width * scale);
        var heartSpacing = 5;
        var startX = 10;
        var startY = 10;

        for (var i = 0; i < _playerController.MaxHealth; i++)
        {
            var position = new Vector2(startX + i * (heartSize + heartSpacing), startY);
            var texture = i < _playerController.CurrentHealth ? _fullHeartTexture : _emptyHeartTexture;

            spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }

    public void UpdateGame(GameTime gameTime)
    {
        var keystate = Keyboard.GetState();
        _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);

        if (!_playerController.IsAlive)
        {
            GameStateManager.Instance.ChangeState(GameState.GameOver);
            return;
        }

        CheckForLevelTransition();
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        var playerRect = _playerController.GetRectangle();

        _camera.X = playerRect.Center.X - _graphics.PreferredBackBufferWidth / 2;
        _camera.Y = playerRect.Center.Y - _graphics.PreferredBackBufferHeight / 2;

        _camera.X = Math.Max(0, _camera.X);
        _camera.Y = Math.Max(0, _camera.Y);
        _camera.X = Math.Min(_currentLevel.Width * _tileSize - _graphics.PreferredBackBufferWidth, _camera.X);
        _camera.Y = Math.Min(_currentLevel.Height * _tileSize - _graphics.PreferredBackBufferHeight, _camera.Y);
    }
    public void ClearEnemies()
    {
        _enemies.Clear();
    }
    private void SpawnWalkerEnemies()
    {
        Console.WriteLine("Spawning WalkerEnemies...");

        var enemySpriteSheet = Content.Load<Texture2D>("walker");

        var addedPositions = new HashSet<Vector2>();

        foreach (var kvp in _currentLevel.Spawns)
        {
            if (kvp.Value == 1) // ID for WalkerEnemies
            {
                var spawnTilePosition = kvp.Key;
                var spawnRect = new Rectangle(
                    (int)spawnTilePosition.X * _tileSize,
                    (int)spawnTilePosition.Y * _tileSize,
                    54, // Width of the enemy sprite
                    35 // Height of the enemy sprite
                );

                if (addedPositions.Add(spawnTilePosition))
                {
                    var walkerEnemy = EnemyFactory.CreateWalkerEnemy(
                        enemySpriteSheet,
                        spawnRect,
                        50, // Example speed
                        _collisionManager,
                        _tileSize,
                        _playerController, // Pass the player controller here
                        new Health(1) // Create a new Health instance for each enemy
                    );


                    _enemies.Add(walkerEnemy);
                    Console.WriteLine($"Spawned WalkerEnemy at: {spawnRect.X}, {spawnRect.Y}");
                }
            }
        }
    }
    
    private void SpawnHiderEnemies()
    {
        Console.WriteLine("Spawning HiderEnemies...");
        
        var hidingTexture = Content.Load<Texture2D>("EggCluster");    // Ensure these are loaded
        var explosionTexture = Content.Load<Texture2D>("Eggsplosion");
        var deathTexture = Content.Load<Texture2D>("EggPopped");

        foreach (var kvp in _currentLevel.Spawns)
        {
            if (kvp.Value == 3) // ID for HiderEnemies
            {
                var spawnTilePosition = kvp.Key;
                var spawnRect = new Rectangle(
                    (int)spawnTilePosition.X * _tileSize,
                    (int)spawnTilePosition.Y * _tileSize,
                    54, // Width of the enemy sprite
                    35  // Height of the enemy sprite
                );
                

                var hiderEnemy = EnemyFactory.CreateHiderEnemy(
                    hidingTexture,
                    explosionTexture,
                    deathTexture,
                    spawnRect,
                    _playerController,   // Pass the player controller here
                    new Health(1)        // Create a new Health instance for each enemy
                );

                _enemies.Add(hiderEnemy);
                Console.WriteLine($"Spawned HiderEnemy at: {spawnRect.X}, {spawnRect.Y}");
            }
        }
    }


    private void DrawTiles(Dictionary<Vector2, int> tiles, Texture2D texture)
    {
        var displayTileSize = _tileSize;

        foreach (var kvp in tiles)
        {
            var position = kvp.Key;
            var tileValue = kvp.Value;

            var destinationRect = new Rectangle(
                (int)(position.X * displayTileSize - _camera.X),
                (int)(position.Y * displayTileSize - _camera.Y),
                displayTileSize,
                displayTileSize
            );

            var sourceRect = new Rectangle(
                tileValue % 20 * displayTileSize,
                tileValue / 20 * displayTileSize,
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