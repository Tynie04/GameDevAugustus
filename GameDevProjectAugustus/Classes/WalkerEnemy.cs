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

    public WalkerEnemy(Texture2D spriteSheet, Rectangle spawnRect, float speed, ICollisionManager collisionManager, int tileSize, IPlayerController playerController)
    {
        _speed = speed;
        _collisionManager = collisionManager;
        _tileSize = tileSize;
        _playerController = playerController;

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
        // Update the animation based on current state
        _animations[_currentState].Update(gameTime);

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
    }

    private void CheckForPlayerCollision()
    {
        if (_playerController == null)
        {
            System.Diagnostics.Debug.WriteLine("PlayerController is null in WalkerEnemy.");
            return;
        }

        var playerRect = _playerController.GetRectangle();

        if (CheckCollision(playerRect))
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                _currentState = State.Attack;
                _playerController.TakeDamage(1);
            }
        }
        else
        {
            _isAttacking = false;
            if (!_isIdling && _currentState != State.Walk)
            {
                _currentState = State.Walk;
            }
        }
    }

    private bool CheckCollision(Rectangle playerRect)
    {
        var enemyRect = new Rectangle((int)_position.X, (int)_position.Y, 54, 35);
        return enemyRect.Intersects(playerRect);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 camera)
    {
        var spriteEffects = _movingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        _animations[_currentState].Draw(spriteBatch, _position - camera, spriteEffects);
    }

    private void Move(GameTime gameTime)
    {
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
}
