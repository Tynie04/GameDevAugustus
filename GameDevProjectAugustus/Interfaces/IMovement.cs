using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus.Interfaces;

public interface IMovement
{
    Vector2 UpdateMovement(Vector2 velocity, KeyboardState keystate);
}