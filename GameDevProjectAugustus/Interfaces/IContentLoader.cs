using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public interface IContentLoader
{
    void LoadContent(ContentManager content);
    Texture2D GetTexture(string textureName);
}