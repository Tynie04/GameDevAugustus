using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Interfaces;

public interface IPhysics
{
    Vector2 ApplyPhysics(Vector2 velocity, float gravity, float maxFallSpeed);
}