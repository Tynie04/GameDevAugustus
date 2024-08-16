using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.UtilClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers;

public class Animation : IAnimation
{
    private Texture2D _texture;
    private List<AnimationFrame> _frames;
    private int _currentFrame;
    private double _frameTime;
    private double _timer;
    private bool _isLooping;

    // Additional properties
    public string Name { get; private set; } // Property to hold the animation name
    public bool IsComplete 
    { 
        get 
        {
            // Animation is complete if it has finished all frames and is not looping
            return !_isLooping && _currentFrame >= _frames.Count - 1;
        }
    }

    public Animation(string name, Texture2D texture, List<AnimationFrame> frames, double frameTime, bool isLooping = true)
    {
        Name = name; // Set the name of the animation
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
                if (_isLooping)
                {
                    _currentFrame = 0; // Loop back to the first frame
                }
                else
                {
                    _currentFrame = _frames.Count - 1; // Stay at the last frame
                }
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
    {
        var sourceRectangle = _frames[_currentFrame].SourceRectangle;
        spriteBatch.Draw(_texture, position, sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, spriteEffects, 0f);
    }
    
}