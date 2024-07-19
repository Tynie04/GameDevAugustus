using Microsoft.Xna.Framework;

public class AnimationFrame
{
    public Rectangle SourceRectangle { get; private set; }

    public AnimationFrame(Rectangle sourceRectangle)
    {
        SourceRectangle = sourceRectangle;
    }
}