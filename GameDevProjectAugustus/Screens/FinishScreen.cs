using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus.Screens;

public class FinishScreen : IScreen
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteFont _font;

    public FinishScreen(GraphicsDeviceManager graphics, SpriteFont font)
    {
        _graphics = graphics;
        _font = font;
    }

    public void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            GameStateManager.Instance.ChangeState(GameState.Start);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _graphics.GraphicsDevice.Clear(Color.Green);
        spriteBatch.DrawString(_font, "Congratulations! You finished all the levels", new Vector2(100, 100), Color.White);
        spriteBatch.DrawString(_font, "Press Enter to Return to level 1", new Vector2(100, 150), Color.White);
        spriteBatch.DrawString(_font, "Press Escape to Exit", new Vector2(100, 200), Color.White);
    }
}