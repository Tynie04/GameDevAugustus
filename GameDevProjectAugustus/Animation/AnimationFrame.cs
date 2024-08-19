using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.UtilClasses;

public class AnimationFrame
{
    public Rectangle SourceRectangle { get; private set; }

    public AnimationFrame(Rectangle sourceRectangle)
    {
        SourceRectangle = sourceRectangle;
    }
}