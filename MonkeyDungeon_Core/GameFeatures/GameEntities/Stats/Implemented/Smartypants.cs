﻿using MonkeyDungeon_Vanilla_Domain.GameFeatures;

namespace MonkeyDungeon_Core.GameFeatures.GameEntities.Stats.Implemented
{
    public class Smartypants : GameEntity_Stat
    {
        public Smartypants(float baseValue, float maxProgresionRate) 
            : base(MD_VANILLA_STATS.STAT_SMARTYPANTS, baseValue, maxProgresionRate)
        {
        }
    }
}