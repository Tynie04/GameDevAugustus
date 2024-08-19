using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.WalkerStates
{
    public class AttackingState : IWalkerState
    {
        public void EnterState(WalkerEnemy walker)
        {
            walker.SetAnimation(State.Attack);
            walker.StartAttack();
        }

        public void Update(WalkerEnemy walker, GameTime gameTime)
        {
            walker.OnPlayerCollision(); // Ensure that attack logic is handled during the update
            if (walker.IsAttackComplete())
            {
                walker.TransitionToState(new WalkingState());
            }
        }

        public void OnPlayerCollision(WalkerEnemy walker)
        {
            // If attacking, do nothing special on collision
        }
    }
}