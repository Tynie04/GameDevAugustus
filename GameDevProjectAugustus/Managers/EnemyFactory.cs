using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameDevProjectAugustus.Interfaces;

namespace GameDevProjectAugustus.Classes
{
    public static class EnemyFactory
    {
        public static IEnemy CreateWalkerEnemy(
            Texture2D spriteSheet,
            Rectangle spawnRect,
            float speed,
            ICollisionManager collisionManager,
            int tileSize,
            IPlayerController playerController) // Added parameter
        {
            return new WalkerEnemy(
                spriteSheet,
                spawnRect,
                speed,
                collisionManager,
                tileSize,
                playerController // Pass the player controller
            );
        }
    }
}