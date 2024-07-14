using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameDevProjectAugustus
{
    public class Sprite
    {
        public Texture2D texture;
        public Rectangle rect;
        public Rectangle srect;
        public Vector2 velocity;
        public bool isGrounded;

        private float gravity = 0.15f;
        private float jumpSpeed = -5f;
        private float moveSpeed = 3f;
        private float maxFallSpeed = 5f;

        public Sprite(Texture2D texture, Rectangle rect, Rectangle srect)
        {
            this.texture = texture;
            this.rect = rect;
            this.srect = srect;
            velocity = Vector2.Zero;
            isGrounded = false;
        }

        public void Update(KeyboardState keystate, Dictionary<Vector2, int> collisions, int tileSize)
        {
            velocity.X = 0;

            if (keystate.IsKeyDown(Keys.Right))
            {
                velocity.X = moveSpeed;
            }
            if (keystate.IsKeyDown(Keys.Left))
            {
                velocity.X = -moveSpeed;
            }
            if (keystate.IsKeyDown(Keys.Up) && isGrounded)
            {
                velocity.Y = jumpSpeed;
                isGrounded = false;
            }

            // Apply gravity
            velocity.Y += gravity;
            if (velocity.Y > maxFallSpeed)
            {
                velocity.Y = maxFallSpeed;
            }

            // Horizontal movement and collision detection
            rect.X += (int)velocity.X;
            HandleHorizontalCollisions(collisions, tileSize);

            // Vertical movement and collision detection
            rect.Y += (int)velocity.Y;
            HandleVerticalCollisions(collisions, tileSize);
        }

        private void HandleHorizontalCollisions(Dictionary<Vector2, int> collisions, int tileSize)
        {
            List<Rectangle> intersectingTiles = GetIntersectingTiles(rect, tileSize, collisions);
            foreach (Rectangle tile in intersectingTiles)
            {
                if (rect.Intersects(tile))
                {
                    if (velocity.X > 0)
                    {
                        rect.X = tile.Left - rect.Width;
                    }
                    else if (velocity.X < 0)
                    {
                        rect.X = tile.Right;
                    }
                    velocity.X = 0;
                }
            }
        }

        private void HandleVerticalCollisions(Dictionary<Vector2, int> collisions, int tileSize)
        {
            List<Rectangle> intersectingTiles = GetIntersectingTiles(rect, tileSize, collisions);
            foreach (Rectangle tile in intersectingTiles)
            {
                if (rect.Intersects(tile))
                {
                    if (velocity.Y > 0)
                    {
                        rect.Y = tile.Top - rect.Height;
                        isGrounded = true;
                    }
                    else if (velocity.Y < 0)
                    {
                        rect.Y = tile.Bottom;
                    }
                    velocity.Y = 0;
                }
            }
        }

        private List<Rectangle> GetIntersectingTiles(Rectangle target, int tileSize, Dictionary<Vector2, int> collisions)
        {
            List<Rectangle> intersectingTiles = new List<Rectangle>();
            int leftTile = target.Left / tileSize;
            int rightTile = target.Right / tileSize;
            int topTile = target.Top / tileSize;
            int bottomTile = target.Bottom / tileSize;

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    Vector2 tilePosition = new Vector2(x, y);
                    if (collisions.ContainsKey(tilePosition))
                    {
                        Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                        intersectingTiles.Add(tileRect);
                    }
                }
            }

            return intersectingTiles;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rect, srect, Color.White);
        }
    }
}
