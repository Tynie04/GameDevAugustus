using System.Collections.Generic;
using GameDevProjectAugustus.UtilClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers
{
    /// <summary>
    /// Factory class for creating animations.
    /// </summary>
    public static class AnimationFactory
    {
        /// <summary>
        /// Creates an animation from a single line of frames in a texture.
        /// </summary>
        /// <param name="texture">The texture containing the animation frames.</param>
        /// <param name="frameWidth">The width of each frame.</param>
        /// <param name="frameHeight">The height of each frame.</param>
        /// <param name="startFrame">The index of the starting frame.</param>
        /// <param name="frameCount">The number of frames in the animation.</param>
        /// <param name="frameTime">The time each frame is displayed.</param>
        /// <param name="isLooping">Indicates whether the animation should loop.</param>
        /// <returns>An instance of the <see cref="Animation"/> class.</returns>
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

        /// <summary>
        /// Creates an animation from multiple lines of frames in a texture.
        /// </summary>
        /// <param name="texture">The texture containing the animation frames.</param>
        /// <param name="frameWidth">The width of each frame.</param>
        /// <param name="frameHeight">The height of each frame.</param>
        /// <param name="startRow">The row where the animation starts.</param>
        /// <param name="startFrame">The index of the starting frame in the row.</param>
        /// <param name="frameCount">The number of frames in the animation.</param>
        /// <param name="frameTime">The time each frame is displayed.</param>
        /// <param name="isLooping">Indicates whether the animation should loop.</param>
        /// <returns>An instance of the <see cref="Animation"/> class.</returns>
        public static Animation CreateAnimationFromMultiLine(Texture2D texture, int frameWidth, int frameHeight, int startRow, int startFrame, int frameCount, double frameTime, bool isLooping = true)
        {
            var frames = new List<AnimationFrame>();
            int columns = texture.Width / frameWidth; // Number of columns in the texture
            int rows = texture.Height / frameHeight;  // Number of rows in the texture

            int currentRow = startRow;
            int frameIndex = startFrame;

            for (int i = 0; i < frameCount; i++)
            {
                int column = frameIndex % columns;
                var frameRectangle = new Rectangle(column * frameWidth, currentRow * frameHeight, frameWidth, frameHeight);
                frames.Add(new AnimationFrame(frameRectangle));

                frameIndex++;

                // Move to the next row if we exceed the number of columns
                if (frameIndex >= columns)
                {
                    frameIndex = 0;
                    currentRow++;
                    if (currentRow >= rows) // Ensure we do not go beyond the number of rows
                    {
                        currentRow = startRow; // Optionally wrap around to startRow
                    }
                }
            }

            return new Animation("AnimationName", texture, frames, frameTime, isLooping); // Provide a default name
        }

    }
}
