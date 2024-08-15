﻿using System;
using System.Collections.Generic;
using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Sprite : IPlayerController
{
    private readonly Dictionary<string, IAnimation> _animations;
    private IAnimation _currentAnimation;
    private readonly IMovement _movement;
    private readonly IPhysics _physics;
    private readonly ICollisionManager _collisionManager;
    private readonly IHealth _health;

    public Rectangle Rect;
    public Vector2 Velocity;
    public bool IsGrounded;
    private bool _facingLeft;
    public bool IsAlive => _health.IsAlive;

    private readonly float _gravity = 0.15f;
    private readonly float _maxFallSpeed = 5f;
    private readonly float _jumpSpeed = -5f;
    private readonly int _tileSize;

    private float _invulnerabilityTimer; // Timer for invulnerability
    private bool _isFlickering; // Flag for flickering
    private bool _isDeathAnimationComplete; // Track death animation completion
    private bool _playHurtAnimation;

    public event EventHandler OnDeath;

    public Sprite(IMovement movement, IPhysics physics, ICollisionManager collisionManager, IHealth health, Rectangle rect, int tileSize)
    {
        _animations = new Dictionary<string, IAnimation>();
        _movement = movement;
        _physics = physics;
        _collisionManager = collisionManager;
        _health = health;
        Rect = rect;
        _tileSize = tileSize;
        Velocity = Vector2.Zero;
        IsGrounded = false;
        _facingLeft = true;
    }

    public void Initialize(Vector2 spawnPoint)
    {
        Rect = new Rectangle((int)spawnPoint.X * _tileSize, (int)spawnPoint.Y * _tileSize, Rect.Width, Rect.Height);
    }

    public void AddAnimation(string name, IAnimation animation)
    {
        _animations[name] = animation;
    }

    public void PlayAnimation(string name)
    {
        if (_animations.ContainsKey(name)) _currentAnimation = _animations[name];
    }

    public void Update(GameTime gameTime, KeyboardState keystate, Level level, int tileSize)
    {
        // Check invulnerability timer
        if (_invulnerabilityTimer > 0)
        {
            _invulnerabilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //PlayAnimation("Hurt");
            // Handle flickering; make the flicker slower
            _isFlickering = _invulnerabilityTimer > 0 && _invulnerabilityTimer % 0.5f < 0.25f; // Flicker every 0.5 seconds
        }
        else
        {
            _isFlickering = false; // Stop flickering when timer ends
        }

        // Update movement (horizontal only)
        Velocity = _movement.UpdateMovement(Velocity, keystate);

        // Determine the facing direction
        _facingLeft = Velocity.X < 0;

        // Handle jumping
        if (keystate.IsKeyDown(Keys.Space) && IsGrounded)
        {
            Velocity.Y = _jumpSpeed;
            IsGrounded = false; // Player is in the air now
            PlayAnimation("Jump"); // Switch to jump animation
        }

        // Apply physics
        Velocity = _physics.ApplyPhysics(Velocity, _gravity, _maxFallSpeed);

        // Handle collisions
        _collisionManager.HandleCollisions(this, level, tileSize);

        // Switch back to idle animation if grounded
        if (IsGrounded && Velocity.Y == 0 &&  !_isFlickering) PlayAnimation("Idle");

        // Update animation
        _currentAnimation?.Update(gameTime);

        // Check if the death animation is complete
        if (!_isDeathAnimationComplete && _currentAnimation?.Name == "Death" && _currentAnimation.IsComplete)
        {
            _isDeathAnimationComplete = true;
            // Optionally, handle post-death logic here if needed
            OnDeath?.Invoke(this, EventArgs.Empty); // Trigger the death event
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition)
    {
        if (_isFlickering)
        {
            // Skip drawing the sprite to create a flickering effect
            return;
        }

        if (_currentAnimation != null)
        {
            var spriteEffects = _facingLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _currentAnimation.Draw(spriteBatch, new Vector2(Rect.X - cameraPosition.X, Rect.Y - cameraPosition.Y), spriteEffects);
        }
    }

    public Rectangle GetRectangle()
    {
        return Rect;
    }

    public void TakeDamage(int amount)
    {
        if (_invulnerabilityTimer > 0) return; // Ignore damage during invulnerability

        _health.TakeDamage(amount);

        Console.WriteLine($"Taking damage: {amount}. Current health: {_health.CurrentHealth}");

        if (!_health.IsAlive)
        {
            Console.WriteLine("Player is dead. Playing death animation.");
            PlayAnimation("Death");
            _isDeathAnimationComplete = false; // Reset flag for animation
            _invulnerabilityTimer = 0f; // Stop invulnerability timer for death
            _isFlickering = false; // Stop flickering on death
            _playHurtAnimation = false; // Ensure hurt animation isn't played on death
        }
        else
        {
            Console.WriteLine("Player hurt. Playing hurt animation.");
            _playHurtAnimation = true; // Set flag to play hurt animation
            _invulnerabilityTimer = 1f; // Set a shorter invulnerability period
        }
    }


    public void Heal(int amount)
    {
        _health.Heal(amount);
    }

    public void ResetHealth()
    {
        if (this._health is Health health)
        {
            health.Heal(3);
        }

        // Reset flickering state and invulnerability timer when starting a new game
        _invulnerabilityTimer = 0f;
        _isFlickering = false;
        _isDeathAnimationComplete = false;
    }
}
