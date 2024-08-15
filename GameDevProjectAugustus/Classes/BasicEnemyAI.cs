using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Classes
{
    public class BasicEnemyAI : IEnemyAi
    {
        public void Update(IEnemy enemy, GameTime gameTime)
        {
            // Basic AI logic for updating the enemy
            // This could involve attacking, patrolling, etc.
        }
    }
}