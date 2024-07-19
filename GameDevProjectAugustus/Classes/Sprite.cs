using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;

public class Sprite : IPlayerController
{
    private Dictionary<string, IAnimation> _animations;
    private IAnimation _currentAnimation;
    private IMovement _movement;
    private IPhysics _physics;
    private ICollisionManager _collisionManager;

    public Rectangle rect;
    public Vector2 velocity;
    public bool isGrounded;
    private bool facingLeft;

    private float gravity = 0.15f;
    private float maxFallSpeed = 5f;
    private float jumpSpeed = -5f;

    public Sprite(IMovement movement, IPhysics physics, ICollisionManager collisionManager, Rectangle rect)
    {
        _animations = new Dictionary<string, IAnimation>();
        _movement = movement;
        _physics = physics;
        _collisionManager = collisionManager;
        this.rect = rect;
        velocity = Vector2.Zero;
        isGrounded = false;
        facingLeft = true; // Assuming the default direction is left
    }

    public void AddAnimation(string name, IAnimation animation)
    {
        _animations[name] = animation;
    }

    public void PlayAnimation(string name)
    {
        if (_animations.ContainsKey(name))
        {
            _currentAnimation = _animations[name];
        }
    }

    public void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize)
    {
        // Update movement (horizontal only)
        velocity = _movement.UpdateMovement(velocity, keystate);

        // Determine the facing direction
        if (velocity.X > 0)
        {
            facingLeft = false;
        }
        else if (velocity.X < 0)
        {
            facingLeft = true;
        }

        // Handle jumping
        if (keystate.IsKeyDown(Keys.Space) && isGrounded)
        {
            velocity.Y = jumpSpeed;
            isGrounded = false; // Player is in the air now
            PlayAnimation("Jump"); // Switch to jump animation
        }

        // Apply physics
        velocity = _physics.ApplyPhysics(velocity, gravity, maxFallSpeed);

        // Handle collisions
        _collisionManager.HandleCollisions(this, level, tileSize);

        // Switch back to idle animation if grounded
        if (isGrounded && velocity.Y == 0)
        {
            PlayAnimation("Idle");
        }

        // Update animation
        _currentAnimation?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_currentAnimation != null)
        {
            SpriteEffects spriteEffects = facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _currentAnimation.Draw(spriteBatch, new Vector2(rect.X, rect.Y), spriteEffects);
        }
    }

    public Rectangle GetRectangle()
    {
        return rect;
    }
}
