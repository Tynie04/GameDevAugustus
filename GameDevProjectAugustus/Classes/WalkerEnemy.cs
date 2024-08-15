using System.Collections.Generic;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WalkerEnemy : IEnemy
{
    private State _currentState;
    private Dictionary<State, IAnimation> _animations;
    private bool _movingRight;
    private float _speed;
    private Vector2 _startPosition;
    private Vector2 _position;
    private ICollisionManager _collisionManager;
    private IPlayerController _playerController;
    private int _tileSize;
    private int _maxMovementRangeInPixels;
    private float _idleDuration = 2f;
    private float _idleTimer;
    private bool _isIdling;
    private bool _isAttacking = false;
    private IHealth _health;

    public bool IsAlive => _health.IsAlive;

    public WalkerEnemy(Texture2D spriteSheet, Rectangle spawnRect, float speed, ICollisionManager collisionManager, int tileSize, IPlayerController playerController, IHealth health)
    {
        _speed = speed;
        _collisionManager = collisionManager;
        _tileSize = tileSize;
        _playerController = playerController;
        _health = health;

        int frameWidth = 54;
        int frameHeight = 35;

        _animations = new Dictionary<State, IAnimation>
        {
            { State.Idle, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 0, 0, 3, 0.2, true) },
            { State.Walk, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 1, 0, 8, 0.2, true) },
            { State.Attack, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 2, 0, 7, 0.2, false) },
            { State.Hurt, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 3, 0, 3, 0.15, false) },
            { State.Death, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 4, 0, 9, 0.1, false) }
        };

        _currentState = State.Walk;
        _position = new Vector2(spawnRect.X, spawnRect.Y);
        _startPosition = _position;
        _movingRight = true;

        _maxMovementRangeInPixels = _tileSize * 2; // Movement range in pixels
    }

    public void Update(GameTime gameTime)
    {
        if (_currentState == State.Death && _animations[State.Death].IsComplete)
        {
            // Death animation complete; walker should not do anything further
            return;
        }

        if (_isIdling)
        {
            _idleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_idleTimer >= _idleDuration)
            {
                _idleTimer = 0f;
                _isIdling = false;
                _currentState = State.Walk; // Switch back to walking after idling
            }
        }
        else
        {
            Move(gameTime);
            CheckForPlayerCollision();
        }

        // Update the animation based on current state
        _animations[_currentState].Update(gameTime);
    }

    private void CheckForPlayerCollision()
    {
        if (_playerController == null)
        {
            System.Diagnostics.Debug.WriteLine("PlayerController is null in WalkerEnemy.");
            return;
        }

        var playerRect = _playerController.GetRectangle();
        var playerVelocity = _playerController.GetVelocity();

        var enemyRect = new Rectangle((int)_position.X, (int)_position.Y, 54, 35);
        var isColliding = enemyRect.Intersects(playerRect);

        if (isColliding)
        {
            // Calculate overlap to determine if the player is landing on top
            bool isPlayerOnTop = playerRect.Bottom <= _position.Y + 10 && playerRect.Bottom > _position.Y && playerVelocity.Y > 0;

            if (isPlayerOnTop)
            {
                // Player landed on the walker
                if (!_health.IsAlive)
                {
                    return;
                }

                // Walker takes damage, play hurt animation
                TakeDamage(1); // Damage the walker
            }
            else if (!_isAttacking)
            {
                // Player touched from the side
                if (_health.IsAlive)
                {
                    _playerController.TakeDamage(1); // Damage the player
                }
            }
        }
    }

    private void Move(GameTime gameTime)
    {
        if (_currentState == State.Death) return; // Do not move if dead

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float movementAmount = _speed * deltaTime;

        // Calculate new position based on direction
        if (_movingRight)
        {
            if (_position.X - _startPosition.X >= _maxMovementRangeInPixels)
            {
                StartIdling();
                _movingRight = false;
            }
            else
            {
                _position.X += movementAmount;
            }
        }
        else
        {
            if (_startPosition.X - _position.X >= _maxMovementRangeInPixels)
            {
                StartIdling();
                _movingRight = true;
            }
            else
            {
                _position.X -= movementAmount;
            }
        }
    }

    private void StartIdling()
    {
        _isIdling = true;
        _currentState = State.Idle;
    }
    
    private void TakeDamage(int amount)
    {
        if (!_health.IsAlive) 
        {
            System.Diagnostics.Debug.WriteLine("Walker already dead, damage ignored.");
            return;
        }

        // Subtract health
        _health.TakeDamage(amount);

        if (!_health.IsAlive)
        {
            _currentState = State.Death;
            _movingRight = false;
            _isIdling = false;
            System.Diagnostics.Debug.WriteLine("Walker died.");
        }
        else
        {
            _currentState = State.Hurt;
            System.Diagnostics.Debug.WriteLine("Walker took damage.");
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 camera)
    {
        var spriteEffects = _movingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        _animations[_currentState].Draw(spriteBatch, _position - camera, spriteEffects);
    }
}
