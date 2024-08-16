using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.States
{
    public class DeathState : IWalkerState
    {
        public void EnterState(WalkerEnemy walker)
        {
            walker.SetAnimation(State.Death);
            walker.HandleDeath();
        }

        public void Update(WalkerEnemy walker, GameTime gameTime)
        {
            // The walker does nothing after death.
        }

        public void OnPlayerCollision(WalkerEnemy walker)
        {
            // If dead, do nothing special on collision.
        }
    }
}