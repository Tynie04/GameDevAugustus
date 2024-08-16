using GameDevProjectAugustus.Classes;
using GameDevProjectAugustus.Enums;
using GameDevProjectAugustus.Interfaces;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.WalkerStates
{
    public class IdleState : IWalkerState
    {
        private float _idleDuration = 2f;
        private float _idleTimer;

        public void EnterState(WalkerEnemy walker)
        {
            _idleTimer = 0f;
            walker.SetAnimation(State.Idle);
        }

        public void Update(WalkerEnemy walker, GameTime gameTime)
        {
            _idleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_idleTimer >= _idleDuration)
            {
                walker.TransitionToState(new WalkingState());
            }
        }

        public void OnPlayerCollision(WalkerEnemy walker)
        {
            // No specific behavior in idle state for player collision
        }
    }
}