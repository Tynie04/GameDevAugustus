using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Animation.Interfaces;

public interface IAnimation
{
    string Name { get; }
    bool IsComplete { get; }
    void Update(GameTime gameTime);
    void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects);
}