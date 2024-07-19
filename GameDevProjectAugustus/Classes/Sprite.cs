using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sprite : IPlayerController
{
    private IAnimation _animation;
    private IMovement _movement;
    private IPhysics _physics;
    private ICollisionManager _collisionManager;

    public Rectangle rect;
    public Vector2 velocity;
    public bool isGrounded;

    private float gravity = 0.15f;
    private float maxFallSpeed = 5f;
    private float jumpSpeed = -5f; // Specific jump speed for the sprite

    public Sprite(IAnimation animation, IMovement movement, IPhysics physics, ICollisionManager collisionManager, Rectangle rect)
    {
        _animation = animation;
        _movement = movement;
        _physics = physics;
        _collisionManager = collisionManager;
        this.rect = rect;
        velocity = Vector2.Zero;
        isGrounded = false;
    }

    public void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize)
    {
        // Update movement (horizontal only)
        velocity = _movement.UpdateMovement(velocity, keystate);

        // Handle jumping
        if (keystate.IsKeyDown(Keys.Space) && isGrounded)
        {
            velocity.Y = jumpSpeed;
            isGrounded = false; // Player is in the air now
        }

        // Apply physics
        velocity = _physics.ApplyPhysics(velocity, gravity, maxFallSpeed);

        // Handle collisions
        _collisionManager.HandleCollisions(this, level, tileSize);

        // Update animation
        _animation.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _animation.Draw(spriteBatch, new Vector2(rect.X, rect.Y));
    }

    public Rectangle GetRectangle()
    {
        return rect;
    }

    public void SetAnimation(IAnimation animation)
    {
        _animation = animation;
    }
}