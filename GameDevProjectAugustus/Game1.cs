using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Dictionary<Vector2, int> ground;
        private Dictionary<Vector2, int> platforms;
        private Dictionary<Vector2, int> collisions;
        private Texture2D texture;
        private Texture2D hitboxTexture;
        private Vector2 camera;

        private Sprite player;
        private int tileSize = 16;

        private Texture2D rectangleTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            ground = LoadMap("../../../Data/testmap_ground.csv");
            platforms = LoadMap("../../../Data/testmap_platforms.csv");
            collisions = LoadMap("../../../Data/testmap_collisions.csv");
            camera = Vector2.Zero;
        }

        private Dictionary<Vector2, int> LoadMap(string filePath)
        {
            Dictionary<Vector2, int> result = new Dictionary<Vector2, int>();
            StreamReader reader = new StreamReader(filePath);

            int y = 0;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(',');

                for (int x = 0; x < items.Length; x++)
                {
                    if (int.TryParse(items[x], out int value))
                    {
                        // Store tile positions only if the value is not -1 (assuming -1 means no tile)
                        if (value != -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }
                }

                y++;
            }

            reader.Close();
            return result;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            texture = Content.Load<Texture2D>("Terrain_and_Props");
            hitboxTexture = Content.Load<Texture2D>("hitbox");

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData(new Color[] { new Color(255, 0, 0, 255) });

            player = new Sprite(
                Content.Load<Texture2D>("player_static"),
                new Rectangle(16, 16, 16, 32),
                new Rectangle(0, 0, 16, 32)
            );
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keystate = Keyboard.GetState();
            player.Update(keystate, collisions, tileSize);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int displayTileSize = 16; // Size of each tile on screen

            foreach (var kvp in ground)
            {
                Vector2 position = kvp.Key;
                int tileValue = kvp.Value;

                Rectangle destinationRect = new Rectangle(
                    (int)position.X * displayTileSize + (int)camera.X,
                    (int)position.Y * displayTileSize + (int)camera.Y,
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

            foreach (var kvp in platforms)
            {
                Vector2 position = kvp.Key;
                int tileValue = kvp.Value;

                Rectangle destinationRect = new Rectangle(
                    (int)position.X * displayTileSize + (int)camera.X,
                    (int)position.Y * displayTileSize + (int)camera.Y,
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

            foreach (var kvp in collisions)
            {
                Vector2 position = kvp.Key;
                int tileValue = kvp.Value;

                Rectangle destinationRect = new Rectangle(
                    (int)position.X * displayTileSize + (int)camera.X,
                    (int)position.Y * displayTileSize + (int)camera.Y,
                    displayTileSize,
                    displayTileSize
                );

                Rectangle sourceRect = new Rectangle(
                    (tileValue % 20) * displayTileSize,
                    (tileValue / 20) * displayTileSize,
                    displayTileSize,
                    displayTileSize
                );

                _spriteBatch.Draw(hitboxTexture, destinationRect, sourceRect, Color.White);
            }

            player.Draw(_spriteBatch);
            DrawRectHollow(_spriteBatch, player.rect, 4);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            spriteBatch.Draw(
                rectangleTexture,
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
