﻿using MonkeyDungeon_Core.GameFeatures.CombatObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyDungeon_Core.GameFeatures.Implemented.Abilities
{
    public class MeleeAbility : GameEntity_Ability
    {
        public MeleeAbility(
            string name, 
            string resourceName, 
            string statName,
            DamageType relevantDamageType = DamageType.Abstract,
            bool requiresTarget = false) 
            : base(name, resourceName, statName, relevantDamageType, requiresTarget)
        {
        }

        protected override void Handle_AbilityUsage(Combat_Action combatAction)
        {
            combatAction.Combat_GameState.Act_MeleeAttack(
                combatAction.Owner_OfCombatAction.Scene_GameObject_ID, 
                combatAction.Target.Scene_GameObject_ID
                );
        }
    }
}