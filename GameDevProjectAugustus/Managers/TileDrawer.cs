using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers
{
    public class TileDrawer
    {
        private readonly int _tileSize;
        private readonly IContentLoader _contentLoader;

        public TileDrawer(int tileSize, IContentLoader contentLoader)
        {
            _tileSize = tileSize;
            _contentLoader = contentLoader;
        }

        public void DrawTiles(SpriteBatch spriteBatch, Dictionary<Vector2, int> tiles, string textureName, Vector2 camera)
        {
            var texture = _contentLoader.GetTexture(textureName);

            foreach (var kvp in tiles)
            {
                Vector2 position = kvp.Key;
                int tileValue = kvp.Value;

                Rectangle destinationRect = new Rectangle(
                    (int)(position.X * _tileSize - camera.X),
                    (int)(position.Y * _tileSize - camera.Y),
                    _tileSize,
                    _tileSize
                );

                Rectangle sourceRect = new Rectangle(
                    (tileValue % 20) * _tileSize,
                    (tileValue / 20) * _tileSize,
                    _tileSize,
                    _tileSize
                );

                spriteBatch.Draw(texture, destinationRect, sourceRect, Color.White);
            }
        }
    }
}