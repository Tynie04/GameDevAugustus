using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Managers;

public class StandardPhysics : IPhysics
{
    public Vector2 ApplyPhysics(Vector2 velocity, float gravity, float maxFallSpeed)
    {
        velocity.Y += gravity;
        if (velocity.Y > maxFallSpeed)
        {
            velocity.Y = maxFallSpeed;
        }
        return velocity;
    }
}