﻿using MonkeyDungeon.GameFeatures.EntityResourceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyDungeon.GameFeatures.Implemented.CharacterStats
{
    public class Agility : GameEntity_Stat
    {
        public Agility(float baseValue, float maxProgresionRate) 
            : base(ENTITY_STATS.AGILITY, baseValue, maxProgresionRate)
        {
        }
    }
}
