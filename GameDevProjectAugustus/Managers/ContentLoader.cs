using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers
{
    public class ContentLoader : IContentLoader
    {
        private readonly Dictionary<string, Texture2D> _textures = new();
    
        public void LoadContent(ContentManager content)
        {
            _textures["Terrain_and_Props"] = content.Load<Texture2D>("Terrain_and_Props");
            _textures["hitbox"] = content.Load<Texture2D>("pixil-frame-0 (4)");
            _textures["Spawns"] = content.Load<Texture2D>("Spawns");
            _textures["Stairs"] = content.Load<Texture2D>("Stairs");
            _textures["Mushroom"] = content.Load<Texture2D>("Mushroom");
        }

        public Texture2D GetTexture(string textureName)
        {
            return _textures[textureName];
        }
    }
}