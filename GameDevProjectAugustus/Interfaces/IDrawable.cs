using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Interfaces;

public interface IDrawable
{
    void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition);
}
