using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Interfaces
{
    public interface IEnemyAi
    {
        void Update(IEnemy enemy, GameTime gameTime);
    }
}