using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Interfaces;

public interface IScreen
{
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch);
}