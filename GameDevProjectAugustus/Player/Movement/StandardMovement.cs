using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus.Managers;

public class StandardMovement : IMovement
{
    private float _moveSpeed;

    public StandardMovement(float moveSpeed)
    {
        _moveSpeed = moveSpeed;
    }

    public Vector2 UpdateMovement(Vector2 velocity, KeyboardState keystate)
    {
        velocity.X = 0;

        if (keystate.IsKeyDown(Keys.Right))
        {
            velocity.X = _moveSpeed;
        }
        if (keystate.IsKeyDown(Keys.Left))
        {
            velocity.X = -_moveSpeed;
        }

        return velocity;
    }
}