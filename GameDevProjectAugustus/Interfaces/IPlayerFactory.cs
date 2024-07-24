using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Interfaces;

public interface IPlayerFactory
{
    Sprite CreatePlayer(Texture2D heroTexture, Vector2 spawnPosition, int tileSize);
}