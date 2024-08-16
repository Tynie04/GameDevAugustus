using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.WalkerStates
{
    public class WalkingState : IWalkerState
    {
        public void EnterState(WalkerEnemy walker)
        {
            walker.SetAnimation(State.Walk);
        }

        public void Update(WalkerEnemy walker, GameTime gameTime)
        {
            walker.Move(gameTime);

            if (walker.ShouldStartIdling())
            {
                walker.TransitionToState(new IdleState());
            }
        }

        public void OnPlayerCollision(WalkerEnemy walker)
        {
            walker.StartAttack();
        }
    }
}