using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;

namespace GameDevProjectAugustus
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ILevelLoader _levelLoader;
        private Level _currentLevel;
        private Texture2D _terrainTexture;
        private Texture2D _hitboxTexture;
        private Vector2 _camera;
        private IPlayerController _playerController;
        private ICollisionManager _collisionManager;
        private int _tileSize = 16;
        private Texture2D _rectangleTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _levelLoader = new CsvLevelLoader("../../../Data");
            _currentLevel = _levelLoader.LoadLevel("testmap");
            _camera = Vector2.Zero;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _terrainTexture = Content.Load<Texture2D>("Terrain_and_Props");
            _hitboxTexture = Content.Load<Texture2D>("hitbox");

            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new Color[] { new Color(255, 0, 0, 255) });

            _playerController = new Sprite(
                Content.Load<Texture2D>("player_static"),
                new Rectangle(16, 16, 16, 32),
                new Rectangle(0, 0, 16, 32)
            );

            _collisionManager = new CollisionManager();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keystate = Keyboard.GetState();
            _playerController.Update(gameTime, keystate, _currentLevel, _tileSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            DrawTiles(_currentLevel.Ground, _terrainTexture);
            DrawTiles(_currentLevel.Platforms, _terrainTexture);
            DrawTiles(_currentLevel.Collisions, _hitboxTexture);

            _playerController.Draw(_spriteBatch);
            DrawRectHollow(_spriteBatch, _playerController.GetRectangle(), 4);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawTiles(Dictionary<Vector2, int> tiles, Texture2D texture)
        {
            int displayTileSize = _tileSize;

            foreach (var kvp in tiles)
            {
                Vector2 position = kvp.Key;
                int tileValue = kvp.Value;

                Rectangle destinationRect = new Rectangle(
                    (int)position.X * displayTileSize + (int)_camera.X,
                    (int)position.Y * displayTileSize + (int)_camera.Y,
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
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                _rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                _rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                _rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }
    }
}
