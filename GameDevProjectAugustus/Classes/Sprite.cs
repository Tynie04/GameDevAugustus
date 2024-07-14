using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sprite : IPlayerController
{
    private Texture2D texture;
    public Rectangle rect;
    private Rectangle srect;
    public Vector2 velocity;
    public bool isGrounded;

    private float gravity = 0.15f;
    private float jumpSpeed = -5f;
    private float moveSpeed = 3f;
    private float maxFallSpeed = 5f;
    
    private ICollisionManager _collisionManager;


    public Sprite(Texture2D texture, Rectangle rect, Rectangle srect)
    {
        this.texture = texture;
        this.rect = rect;
        this.srect = srect;
        velocity = Vector2.Zero;
        isGrounded = false;
        _collisionManager = new CollisionManager();
    }

    public void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize)
    {
        UpdateMovement(keystate);
        ApplyPhysics();
        HandleCollisions(level, tileSize);
    }

    private void UpdateMovement(KeyboardState keystate)
    {
        velocity.X = 0;

        if (keystate.IsKeyDown(Keys.Right))
        {
            velocity.X = moveSpeed;
        }
        if (keystate.IsKeyDown(Keys.Left))
        {
            velocity.X = -moveSpeed;
        }
        if (keystate.IsKeyDown(Keys.Up) && isGrounded)
        {
            velocity.Y = jumpSpeed;
            isGrounded = false;
        }
    }

    private void ApplyPhysics()
    {
        velocity.Y += gravity;
        if (velocity.Y > maxFallSpeed)
        {
            velocity.Y = maxFallSpeed;
        }
    }
    
    public Rectangle GetRectangle()
    {
        return rect;
    }

    private void HandleCollisions(Level level, int tileSize)
    {
        // Assuming a collision manager is used to handle collisions
        _collisionManager.HandleCollisions(this, level, tileSize);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, rect, srect, Color.White);
    }
}