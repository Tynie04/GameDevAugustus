using System.Collections.Generic;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using GameDevProjectAugustus.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class HiderEnemy : IEnemy
{
    private State _currentState;
    private Dictionary<State, IAnimation> _animations;
    private Vector2 _position;
    private IPlayerController _playerController;
    private IHealth _health;
    private float _detectionRadius;

    public bool IsAlive => _health.IsAlive;

    public HiderEnemy(Texture2D hidingTexture, Texture2D explosionTexture, Texture2D deathTexture, Rectangle spawnRect, IPlayerController playerController, IHealth health)
    {
        _playerController = playerController;
        _health = health;

        int explosionFrameWidth = 32;
        int explosionFrameHeight = 32;

        _animations = new Dictionary<State, IAnimation>
        {
            { State.Idle, AnimationFactory.CreateAnimationFromSingleLine(hidingTexture, hidingTexture.Width, hidingTexture.Height, 0, 1, 1.0, true) },
            { State.Attack, AnimationFactory.CreateAnimationFromMultiLine(explosionTexture, explosionFrameWidth, explosionFrameHeight, 0, 0, 8, 0.5, false) },
            { State.Death, AnimationFactory.CreateAnimationFromSingleLine(deathTexture, deathTexture.Width, deathTexture.Height, 0, 1, 1.0, false) }
        };

        _currentState = State.Idle;
        _position = new Vector2(spawnRect.X, spawnRect.Y);
        _detectionRadius = 100f; // Set the detection radius as needed
    }

    public void Update(GameTime gameTime)
    {
        if (_currentState == State.Death && _animations[State.Death].IsComplete)
        {
            return;
        }

        CheckForPlayerCollision();
        _animations[_currentState].Update(gameTime);
    }

    private void CheckForPlayerCollision()
    {
        if (_playerController == null)
        {
            System.Diagnostics.Debug.WriteLine("PlayerController is null in HiderEnemy.");
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

    public void TransitionToAttack()
    {
        if (_currentState != State.Death)
        {
            _currentState = State.Attack;
        }
    }

    public void TransitionToDeath()
    {
        if (_currentState != State.Death)
        {
            _currentState = State.Death;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 camera)
    {
        _animations[_currentState].Draw(spriteBatch, _position - camera, SpriteEffects.None);
    }
}
