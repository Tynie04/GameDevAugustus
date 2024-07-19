using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;

public class Animation : IAnimation
{
    private Texture2D _texture;
    private List<AnimationFrame> _frames;
    private int _currentFrame;
    private double _frameTime;
    private double _timer;
    private bool _isLooping;

    public Animation(Texture2D texture, List<AnimationFrame> frames, double frameTime, bool isLooping = true)
    {
        _texture = texture;
        _frames = frames;
        _frameTime = frameTime;
        _isLooping = isLooping;
        _currentFrame = 0;
        _timer = 0;
    }

    public void Update(GameTime gameTime)
    {
        _timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= _frameTime)
        {
            _timer = 0;
            _currentFrame++;
            if (_currentFrame >= _frames.Count)
            {
                _currentFrame = _isLooping ? 0 : _frames.Count - 1;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
    {
        var sourceRectangle = _frames[_currentFrame].SourceRectangle;
        spriteBatch.Draw(_texture, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
    }
}