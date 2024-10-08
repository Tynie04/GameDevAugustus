﻿using GameDevProjectAugustus.Classes;
using Microsoft.Xna.Framework;

namespace GameDevProjectAugustus.Interfaces
{
    public interface IWalkerState
    {
        void EnterState(WalkerEnemy walker);
        void Update(WalkerEnemy walker, GameTime gameTime);
    }
}