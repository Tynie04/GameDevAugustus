using System.Collections.Generic;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers;

public class PlayerFactory : IPlayerFactory
{
    public Sprite CreatePlayer(Texture2D heroTexture, Vector2 spawnPosition, int tileSize)
    {
        var animations = CreateAnimations(heroTexture);
        var movement = new StandardMovement(3f);
        var physics = new StandardPhysics();
        var collisionManager = new CollisionManager();

        var playerRect = new Rectangle((int)spawnPosition.X, (int)spawnPosition.Y, 32, 32);
        var player = new Sprite(movement, physics, collisionManager, playerRect, tileSize);

        player.Initialize(spawnPosition);

        foreach (var animation in animations)
        {
            player.AddAnimation(animation.Key, animation.Value);
        }

        player.PlayAnimation("Idle");

        return player;
    }

    private Dictionary<string, Animation> CreateAnimations(Texture2D heroTexture)
    {
        return new Dictionary<string, Animation>
        {
            { "Idle", AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 0, 4, 0.2f) },
            { "Jump", AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 4, 11, 0.1f) },
            { "Attack", AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 15, 5, 0.1f) },
            { "Hurt", AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 20, 4, 0.2f) },
            { "Death", AnimationFactory.CreateAnimationFromSingleLine(heroTexture, 32, 32, 24, 9, 0.15f) }
        };
    }
}
