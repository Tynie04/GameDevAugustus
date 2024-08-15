using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

public class AnimationFactory
{
    public static Animation CreateAnimationFromSingleLine(Texture2D texture, int frameWidth, int frameHeight, int startFrame, int frameCount, double frameTime, bool isLooping = true)
    {
        var frames = new List<AnimationFrame>();
        for (int i = 0; i < frameCount; i++)
        {
            var frameRectangle = new Rectangle((startFrame + i) * frameWidth, 0, frameWidth, frameHeight);
            frames.Add(new AnimationFrame(frameRectangle));
        }
        return new Animation("AnimationName", texture, frames, frameTime, isLooping); // Provide a default name
    }

    public static Animation CreateAnimationFromMultiLine(Texture2D texture, int frameWidth, int frameHeight, int rows, int columns, int startFrame, int frameCount, double frameTime, bool isLooping = true)
    {
        var frames = new List<AnimationFrame>();
        int totalFrames = rows * columns;
        for (int i = 0; i < frameCount; i++)
        {
            int frameIndex = (startFrame + i) % totalFrames;
            int row = frameIndex / columns;
            int column = frameIndex % columns;
            var frameRectangle = new Rectangle(column * frameWidth, row * frameHeight, frameWidth, frameHeight);
            frames.Add(new AnimationFrame(frameRectangle));
        }
        return new Animation("AnimationName", texture, frames, frameTime, isLooping); // Provide a default name
    }
}