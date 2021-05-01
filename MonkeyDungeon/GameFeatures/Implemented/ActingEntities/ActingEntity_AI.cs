﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyDungeon.Components;
using MonkeyDungeon.GameFeatures.Implemented.Abilities;
using MonkeyDungeon.GameFeatures.Implemented.GameStates;

namespace MonkeyDungeon.GameFeatures.Implemented.ActingEntities
{
    public class ActingEntity_AI : EntityController
    {
        public ActingEntity_AI()
            : base(true)
        { }

        protected override CombatAction Handle_CombatAction_Request(Combat_GameState combat)
        {
            Random rand = new Random();
            EntityComponent[] players = combat.ConsciousPlayers;
            EntityComponent target = players[rand.Next(players.Length)];

            //TODO: make combat ref GameScene
            CombatAction ca = new CombatAction(combat.GameWorld.GameScene);
            ca.Owner_OfCombatAction = Entity;
            ca.Target = target;
            ca.Set_Ability(Ability_Punch.NAME_PUNCH);

            return ca;
        }
    }
}
