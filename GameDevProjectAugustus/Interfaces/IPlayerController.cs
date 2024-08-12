﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProjectAugustus.Interfaces;

public interface IPlayerController
{
    void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize);
    void Draw(SpriteBatch spriteBatch, Vector2 camera);
    Rectangle GetRectangle();
    void Initialize(Vector2 position);
}