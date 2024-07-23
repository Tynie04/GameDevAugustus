using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public interface IPlayerController
{
    void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize);
    void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition);
    Rectangle GetRectangle();
}