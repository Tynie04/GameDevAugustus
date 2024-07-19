using Microsoft.Xna.Framework;

public interface IPhysics
{
    Vector2 ApplyPhysics(Vector2 velocity, float gravity, float maxFallSpeed);
}