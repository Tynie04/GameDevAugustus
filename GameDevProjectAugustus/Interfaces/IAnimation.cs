using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public interface IAnimation
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position);
}