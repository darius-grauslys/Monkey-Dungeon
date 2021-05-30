﻿using MonkeyDungeon_Vanilla_Domain.GameFeatures;
using System;

namespace MonkeyDungeon_Core.GameFeatures.GameEntities.Abilities.Implemented
{
    public class Ability_ChaosBolt : Ability_Ranged
    {
        private Random rand = new Random();

        public Ability_ChaosBolt() 
            : base(
                  MD_VANILLA_ABILITYNAMES.ABILITY_CHAOS_BOLT, 
                  MD_VANILLA_RESOURCES.RESOURCE_MANA, 
                  MD_VANILLA_STATS.STAT_SMARTYPANTS,
                  MD_VANILLA_PARTICLES.CHAOS_BOLT,
                  Combat_Damage_Type.Magical, 
                  true)
        {
        }

        public override GameEntity_Ability Clone()
        {
            return new Ability_ChaosBolt();
        }
    }
}