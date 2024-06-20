using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus;

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
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (Keyboard.GetState().IsKeyDown(Keys.Right))
            camera.X -= 5;

        if (Keyboard.GetState().IsKeyDown(Keys.Left))
            camera.X += 5;

        // TODO: Add your update logic here

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

            // Calculate destination rectangle
            Rectangle destinationRect = new Rectangle(
                (int)position.X * displayTileSize + (int)camera.X, // X position on screen
                (int)position.Y * displayTileSize + (int)camera.Y, // Y position on screen
                displayTileSize, // Width of the tile on screen
                displayTileSize // Height of the tile on screen
            );

            // Draw the tile using a single source rectangle assuming your tileset is structured
            // such that all tiles are the same size and aligned properly
            Rectangle sourceRect = new Rectangle(
                (tileValue % 20) * displayTileSize, // X position in tileset (assuming 20 tiles wide)
                (tileValue / 20) * displayTileSize, // Y position in tileset (assuming 20 tiles wide)
                displayTileSize, // Width of the tile in tileset
                displayTileSize // Height of the tile in tileset
            );

            // Draw the tile
            _spriteBatch.Draw(texture, destinationRect, sourceRect, Color.White);
        }
        foreach (var kvp in platforms)
        {
            Vector2 position = kvp.Key;
            int tileValue = kvp.Value;

            // Calculate destination rectangle
            Rectangle destinationRect = new Rectangle(
                (int)position.X * displayTileSize + (int)camera.X, // X position on screen
                (int)position.Y * displayTileSize + (int)camera.Y, // Y position on screen
                displayTileSize, // Width of the tile on screen
                displayTileSize // Height of the tile on screen
            );

            // Draw the tile using a single source rectangle assuming your tileset is structured
            // such that all tiles are the same size and aligned properly
            Rectangle sourceRect = new Rectangle(
                (tileValue % 20) * displayTileSize, // X position in tileset (assuming 20 tiles wide)
                (tileValue / 20) * displayTileSize, // Y position in tileset (assuming 20 tiles wide)
                displayTileSize, // Width of the tile in tileset
                displayTileSize // Height of the tile in tileset
            );

            // Draw the tile
            _spriteBatch.Draw(texture, destinationRect, sourceRect, Color.White);
        }
        foreach (var kvp in collisions)
        {
            Vector2 position = kvp.Key;
            int tileValue = kvp.Value;

            // Calculate destination rectangle
            Rectangle destinationRect = new Rectangle(
                (int)position.X * displayTileSize + (int)camera.X, // X position on screen
                (int)position.Y * displayTileSize + (int)camera.Y, // Y position on screen
                displayTileSize, // Width of the tile on screen
                displayTileSize // Height of the tile on screen
            );

            // Draw the tile using a single source rectangle assuming your tileset is structured
            // such that all tiles are the same size and aligned properly
            Rectangle sourceRect = new Rectangle(
                (tileValue % 20) * displayTileSize, // X position in tileset (assuming 20 tiles wide)
                (tileValue / 20) * displayTileSize, // Y position in tileset (assuming 20 tiles wide)
                displayTileSize, // Width of the tile in tileset
                displayTileSize // Height of the tile in tileset
            );

            // Draw the tile
            _spriteBatch.Draw(hitboxTexture, destinationRect, sourceRect, Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

}