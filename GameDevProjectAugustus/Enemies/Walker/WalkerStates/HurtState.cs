using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.WalkerStates
{
    public class HurtState : IWalkerState
    {
        public void EnterState(WalkerEnemy walker)
        {
            walker.SetAnimation(State.Hurt);
            // Optionally handle hurt logic here
        }

        public void Update(WalkerEnemy walker, GameTime gameTime)
        {
            if (walker.IsHurtAnimationComplete())
            {
                if (!walker.IsAlive)
                {
                    walker.TransitionToState(new DeathState());
                }
                else
                {
                    walker.TransitionToState(new WalkingState());
                }
            }
        }

        public void OnPlayerCollision(WalkerEnemy walker)
        {
            // If hurt, do nothing special on collision
        }
    }
}