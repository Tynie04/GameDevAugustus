using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Interfaces;

public interface IEnemy
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition);
}