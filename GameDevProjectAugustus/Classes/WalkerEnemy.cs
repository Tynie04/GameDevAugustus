using System.Collections.Generic;
using System.Diagnostics;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using GameDevProjectAugustus.WalkerStates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Classes;

public class WalkerEnemy : IEnemy
{
    private IWalkerState _currentState;
    private readonly Dictionary<State, IAnimation> _animations;
    private bool _movingRight;
    private readonly float _speed;
    private readonly Vector2 _startPosition;
    private Vector2 _position;
    private ICollisionManager _collisionManager;
    private readonly IPlayerController _playerController;
    private readonly int _maxMovementRangeInPixels;
    private readonly IHealth _health;
    private bool _isIdling;

    public bool IsAlive => _health.IsAlive;

    public WalkerEnemy(Texture2D spriteSheet, Rectangle spawnRect, float speed, ICollisionManager collisionManager, int tileSize, IPlayerController playerController, IHealth health)
    {
        _speed = speed;
        _collisionManager = collisionManager;
        _playerController = playerController;
        _health = health;

        int frameWidth = 54;
        int frameHeight = 35;

        _animations = new Dictionary<State, IAnimation>
        {
            { State.Idle, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 0, 0, 3, 0.2) },
            { State.Walk, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 1, 0, 8, 0.2) },
            { State.Attack, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 2, 0, 7, 0.2, false) },
            { State.Hurt, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 3, 0, 3, 0.15, false) },
            { State.Death, AnimationFactory.CreateAnimationFromMultiLine(spriteSheet, frameWidth, frameHeight, 4, 0, 9, 0.1, false) }
        };

        _position = new Vector2(spawnRect.X, spawnRect.Y);
        _startPosition = _position;
        _movingRight = true;
        _isIdling = false;

        _maxMovementRangeInPixels = tileSize * 2;

        TransitionToState(new WalkingState());
    }

    public void TransitionToState(IWalkerState newState)
    {
        _currentState = newState;
        _currentState.EnterState(this);
    }

    public void Update(GameTime gameTime)
    {
        // Avoid unnecessary updates if dead
        if (_currentState is DeathState && _animations[State.Death].IsComplete)
        {
            return; // Do nothing if dead and death animation is complete
        }

        if (_currentState is HurtState && _animations[State.Hurt].IsComplete)
        {
            // After HurtState, either transition to DeathState or WalkingState
            if (!IsAlive)
            {
                TransitionToState(new DeathState());
            }
            else
            {
                TransitionToState(new WalkingState());
            }
        }

        // Regular update logic
        _currentState?.Update(this, gameTime);
        _animations[GetCurrentState()].Update(gameTime);

        if (!_isIdling)
        {
            Move(gameTime);
        }

        OnPlayerCollision();  // Ensure this is called after movement
    }

    public void OnPlayerCollision()
    {
        if (_playerController == null)
        {
            Debug.WriteLine("PlayerController is null in WalkerEnemy.");
            return;
        }

        var playerRect = _playerController.GetRectangle();
        var playerVelocity = _playerController.GetVelocity();
        var enemyRect = new Rectangle((int)_position.X, (int)_position.Y, 54, 35);

        if (enemyRect.Intersects(playerRect))
        {
            bool isPlayerLandingOnTop = playerRect.Bottom <= _position.Y + 10 && playerRect.Bottom > _position.Y && playerVelocity.Y > 0;

            if (isPlayerLandingOnTop)
            {
                if (IsAlive && !(_currentState is HurtState)) // Prevent re-entrance to HurtState
                {
                    TakeDamage(1); // Walker takes damage
                }
            }
            else
            {
                if (IsAlive && !(_currentState is HurtState) && !_playerController.IsInvulnerable) // Check if player is invulnerable
                {
                    _playerController.TakeDamage(1); // Player takes damage
                }
            }
        }
    }

    public void Move(GameTime gameTime)
    {
        if (_currentState is DeathState) return; // Do not move if dead

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float movementAmount = _speed * deltaTime;

        if (_movingRight)
        {
            if (_position.X - _startPosition.X >= _maxMovementRangeInPixels)
            {
                _movingRight = false;
                _isIdling = true;
                TransitionToState(new IdleState());
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
                _movingRight = true;
                _isIdling = true;
                TransitionToState(new IdleState());
            }
            else
            {
                _position.X -= movementAmount;
            }
        }
    }

    public void StartAttack()
    {
        if (_currentState is AttackingState || !IsAlive)
        {
            return; // Don't start an attack if already attacking or if the walker is dead
        }

        TransitionToState(new AttackingState()); // Transition to attack state
    }

    public bool IsAttackComplete()
    {
        return _animations[State.Attack].IsComplete;
    }

    public void HandleDeath()
    {
        if (!IsAlive && !(_currentState is DeathState))
        {
            TransitionToState(new DeathState()); // Transition to death state
            _isIdling = true; // This prevents movement or attacking
        }
    }

    public bool IsHurtAnimationComplete()
    {
        return _animations[State.Hurt].IsComplete;
    }

    public bool ShouldStartIdling()
    {
        return !_isIdling && (_position.X - _startPosition.X >= _maxMovementRangeInPixels || _startPosition.X - _position.X >= _maxMovementRangeInPixels);
    }

    public void SetAnimation(State state)
    {
        if (_animations.ContainsKey(state))
        {
            _animations[state].Update(new GameTime()); 
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 camera)
    {
        var spriteEffects = _movingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        _animations[GetCurrentState()].Draw(spriteBatch, _position - camera, spriteEffects);
    }

    private State GetCurrentState()
    {
        if (_currentState is IdleState) return State.Idle;
        if (_currentState is WalkingState) return State.Walk;
        if (_currentState is AttackingState) return State.Attack;
        if (_currentState is HurtState) return State.Hurt;
        if (_currentState is DeathState) return State.Death;

        return State.Idle;
    }

    public void TakeDamage(int damage)
    {
        _health.TakeDamage(damage);

        if (!IsAlive)
        {
            // Ensure we only transition to DeathState once
            if (!(_currentState is DeathState))
            {
                HandleDeath(); // Trigger death handling
            }
        }
        else if (!(_currentState is HurtState))
        {
            // Transition to HurtState only if not already in it
            TransitionToState(new HurtState());
        }
    }
}