﻿using MonkeyDungeon_Vanilla_Domain.GameFeatures;

namespace MonkeyDungeon_Core.GameFeatures.GameEntities.Abilities.Implemented
{
    public class Ability_KiKick : GameEntity_Ability
    {
        public Ability_KiKick() 
            : base(
                  MD_VANILLA_ABILITYNAMES.ABILITY_KI_KICK, 
                  MD_VANILLA_RESOURCES.RESOURCE_STAMINA, 
                  MD_VANILLA_STATS.STAT_AGILITY
                  )
        {
        }

        public override GameEntity_Ability Clone()
        {
            return new Ability_KiKick();
        }
    }
}