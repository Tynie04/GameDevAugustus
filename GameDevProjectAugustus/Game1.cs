using GameDevProjectAugustus.Classes;
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

    private IContentLoader _contentLoader;
    private ILevelLoader _levelLoader;
    private ICamera _camera;
    private IPlayerFactory _playerFactory;
    private IPlayerController _playerController;

    private Level _currentLevel;
    private TileDrawer _tileDrawer;

    private int _tileSize = 16;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _contentLoader = new ContentLoader();
        _levelLoader = new CsvLevelLoader("../../../Data");
        _camera = new Camera();
        _playerFactory = new PlayerFactory();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _contentLoader.LoadContent(Content);

        _currentLevel = _levelLoader.LoadLevel("level1");

        var heroTexture = _contentLoader.GetTexture("Mushroom");
        Vector2 spawnPosition = FindSpawnPosition(2);
        _playerController = _playerFactory.CreatePlayer(heroTexture, spawnPosition, _tileSize);

        _tileDrawer = new TileDrawer(_tileSize, _contentLoader);
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
        return Vector2.Zero;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _playerController.Update(gameTime, Keyboard.GetState(), _currentLevel, _tileSize);
        _camera.Update(_playerController.GetRectangle(), _currentLevel.Width * _tileSize, _currentLevel.Height * _tileSize, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        DrawLayers();
        _playerController.Draw(_spriteBatch, _camera.Position);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawLayers()
    {
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Spawns, "Spawns", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Collisions, "hitbox", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Ground, "Terrain_and_Props", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Water, "Terrain_and_Props", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Platforms, "Terrain_and_Props", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Finish, "Stairs", _camera.Position);
        _tileDrawer.DrawTiles(_spriteBatch, _currentLevel.Props, "Terrain_and_Props", _camera.Position);
    }
}
