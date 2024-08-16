using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProjectAugustus.Managers
{
    public static class EnemyFactory
    {
        public static IEnemy CreateWalkerEnemy(
            Texture2D spriteSheet,
            Rectangle spawnRect,
            float speed,
            ICollisionManager collisionManager,
            int tileSize,
            IPlayerController playerController,
            IHealth health)
        {
            return new WalkerEnemy(spriteSheet, spawnRect, speed, collisionManager, tileSize, playerController, health);
        }

        public static IEnemy CreateHiderEnemy(
            Texture2D hidingTexture,
            Texture2D explosionTexture,
            Texture2D deathTexture,
            Rectangle spawnRect,
            IPlayerController playerController,
            IHealth health)
        {
            return new HiderEnemy(hidingTexture, explosionTexture, deathTexture, spawnRect, playerController, health);
        }
    }
}