using System.Collections.Generic;
using Microsoft.Xna.Framework;

public class CollisionManager : ICollisionManager
{
    public void HandleCollisions(Sprite sprite, Level level, int tileSize)
    {
        // Horizontal movement and collision detection
        sprite.rect.X += (int)sprite.velocity.X;
        HandleHorizontalCollisions(sprite, level, tileSize);

        // Vertical movement and collision detection
        sprite.rect.Y += (int)sprite.velocity.Y;
        HandleVerticalCollisions(sprite, level, tileSize);
    }

    private static void HandleHorizontalCollisions(Sprite sprite, Level level, int tileSize)
    {
        List<Rectangle> intersectingTiles = GetIntersectingTiles(sprite.rect, tileSize, level.Collisions);
        foreach (Rectangle tile in intersectingTiles)
        {
            if (sprite.rect.Intersects(tile))
            {
                if (sprite.velocity.X > 0)
                {
                    sprite.rect.X = tile.Left - sprite.rect.Width;
                }
                else if (sprite.velocity.X < 0)
                {
                    sprite.rect.X = tile.Right;
                }
                sprite.velocity.X = 0;
            }
        }
    }

    private static void HandleVerticalCollisions(Sprite sprite, Level level, int tileSize)
    {
        List<Rectangle> intersectingTiles = GetIntersectingTiles(sprite.rect, tileSize, level.Collisions);
        foreach (Rectangle tile in intersectingTiles)
        {
            if (sprite.rect.Intersects(tile))
            {
                if (sprite.velocity.Y > 0)
                {
                    sprite.rect.Y = tile.Top - sprite.rect.Height;
                    sprite.isGrounded = true;
                }
                else if (sprite.velocity.Y < 0)
                {
                    sprite.rect.Y = tile.Bottom;
                }
                sprite.velocity.Y = 0;
            }
        }
    }

    private static List<Rectangle> GetIntersectingTiles(Rectangle target, int tileSize, Dictionary<Vector2, int> collisions)
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
}
