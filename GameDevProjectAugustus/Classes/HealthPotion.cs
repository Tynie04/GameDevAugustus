using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameDevProjectAugustus.Classes
{
    public class HealthPotion
    {
        private Texture2D _texture;
        private Rectangle _rectangle;
        private bool _isActive;

        private float _amplitude; // The height of the oscillation
        private float _speed; // The speed of the oscillation
        private float _time; // To track the time for oscillation
        private int _initialY; // Store the initial Y position

        public HealthPotion(Texture2D texture, Rectangle rectangle, float amplitude = 2f, float speed = 8f)
        {
            _texture = texture;
            _rectangle = rectangle;
            _isActive = true;
            _amplitude = amplitude;
            _speed = speed;
            _time = 0f; // Initialize time
            _initialY = rectangle.Y; // Store the initial Y position
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
        {
            if (!_isActive) return;

            // Draw the potion with the current position adjusted for the camera
            spriteBatch.Draw(_texture, new Vector2(_rectangle.X - cameraPosition.X, _rectangle.Y - cameraPosition.Y), Color.White);
        }

        public void Update(Sprite player, GameTime gameTime)
        {
            if (!_isActive) return;

            // Update the potion's position
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds * _speed;
            float offsetY = (float)Math.Sin(_time) * _amplitude;

            // Apply the oscillation offset to the initial Y position
            _rectangle.Y = _initialY + (int)offsetY;

            var playerRect = player.GetRectangle();

            if (playerRect.Intersects(_rectangle))
            {
                if (player.CurrentHealth < player.MaxHealth)
                {
                    player.Heal(3);
                    _isActive = false; // Potion disappears after being picked up
                }
            }
        }

        public bool IsActive => _isActive;
    }
}
