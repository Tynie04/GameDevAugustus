using System.Collections.Generic;
using System.Diagnostics;
using GameDevProjectAugustus.Animation.Interfaces;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Classes;

public class HiderEnemy : IEnemy
{
    private State _currentState;
    private Dictionary<State, IAnimation> _animations;
    private Vector2 _position;
    private IPlayerController _playerController;
    private IHealth _health;
    private float _detectionRadius;
    private bool _hasDamagedPlayer;
    private readonly float _attackDuration; // Duration of the attack window
    private float _attackTimer; // Timer for tracking the attack duration
    
    public HiderEnemy(Texture2D hidingTexture, Texture2D explosionTexture, Texture2D deathTexture, Rectangle spawnRect, IPlayerController playerController, IHealth health)
    {
        _playerController = playerController;
        _health = health;

        int explosionFrameWidth = 32;
        int explosionFrameHeight = 32;

        _animations = new Dictionary<State, IAnimation>
        {
            { State.Idle, AnimationFactory.CreateAnimationFromSingleLine(hidingTexture, hidingTexture.Width, hidingTexture.Height, 0, 1, 1.0) },
            { State.Attack, AnimationFactory.CreateAnimationFromMultiLine(explosionTexture, explosionFrameWidth, explosionFrameHeight, 0, 0, 8, 0.1, false) },
            { State.Death, AnimationFactory.CreateAnimationFromSingleLine(deathTexture, deathTexture.Width, deathTexture.Height, 0, 1, 1.0, false) }
        };

        _currentState = State.Idle;
        _position = new Vector2(spawnRect.X, spawnRect.Y);
        _detectionRadius = 100f; // Set the detection radius as needed
        _hasDamagedPlayer = false;
        _attackDuration = 0.5f; // Set duration for attack window (in seconds)
        _attackTimer = 0f;
    }

    public void Update(GameTime gameTime)
    {
        // Update animation state
        _animations[_currentState].Update(gameTime);

        switch (_currentState)
        {
            case State.Death:
                break;
            case State.Attack:
            {
                _attackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Check if the player is still in the kill zone and deal damage
                if (_attackTimer >= _attackDuration)
                {
                    if (!_hasDamagedPlayer)
                    {
                        DealDamageToPlayer();
                        _hasDamagedPlayer = true; // Ensure damage is only dealt once per attack
                    }
                }

                // Transition to death state after attack animation is complete
                if (_animations[State.Attack].IsComplete)
                {
                    TransitionToDeath(); // Transition to death animation
                }

                break;
            }
            default:
                CheckForPlayerCollision();
                break;
        }
    }

    private void CheckForPlayerCollision()
    {
        if (_playerController == null)
        {
            Debug.WriteLine("PlayerController is null in HiderEnemy.");
            return;
        }

        Rectangle playerRect = _playerController.GetRectangle();
        Vector2 playerCenter = new Vector2(playerRect.Center.X, playerRect.Center.Y);
        Vector2 enemyCenter = _position;

        float distanceToPlayer = Vector2.Distance(playerCenter, enemyCenter);

        if (distanceToPlayer <= _detectionRadius)
        {
            // Player is within detection radius
            if (_currentState == State.Idle)
            {
                TransitionToAttack();
            }
        }
        else
        {
            // Player is out of detection radius
            if (_currentState != State.Idle && _currentState != State.Death)
            {
                _currentState = State.Idle;
            }
        }
    }

    private void DealDamageToPlayer()
    {
        if (_playerController != null && _currentState == State.Attack)
        {
            _playerController.TakeDamage(2); // Deal 2 damage to the player
        }
    }

    public void TransitionToAttack()
    {
        if (_currentState != State.Death)
        {
            _currentState = State.Attack;
            _hasDamagedPlayer = false; // Reset flag for the new attack
            _attackTimer = 0f; // Reset attack timer
        }
    }

    public void TransitionToDeath()
    {
        
            _currentState = State.Death;
        
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 camera)
    {
        _animations[_currentState].Draw(spriteBatch, _position - camera, SpriteEffects.None);
    }
}