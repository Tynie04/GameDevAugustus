﻿using GameDevProjectAugustus.Classes;

namespace GameDevProjectAugustus.Interfaces
{
    public interface ILevelLoader
    {
        Level LoadLevel(string levelName);
    }
}