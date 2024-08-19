using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Interfaces;

public interface IScreen
{
    void Update();
    void Draw(SpriteBatch spriteBatch);
}