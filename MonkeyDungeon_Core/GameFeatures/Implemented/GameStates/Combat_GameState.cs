﻿using MonkeyDungeon_Core.GameFeatures.CombatObjects;
using MonkeyDungeon_Core.GameFeatures.Implemented.Entities.Enemies.Goblins;
using MonkeyDungeon_Core.GameFeatures.Multiplayer.Handlers;
using MonkeyDungeon_Core.GameFeatures.Multiplayer.MessageWrappers;
using MonkeyDungeon_Vanilla_Domain.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonkeyDungeon_Core.GameFeatures.Implemented.GameStates
{
    public enum CombatState
    {
        BeginNextTurn,
        PlayCurrentTurn,
        FinishCurrentTurn,
        FinishCombat,
        OutOfCombat
    }

    public class Combat_GameState : GameState
    {
        public CombatState CombatState { get; private set; }
        
        /// <summary>
        /// The current turn of the entities.
        /// </summary>
        public int TurnIndex { get; private set; }
        
        /// <summary>
        /// If 1 - normal turn progresion. If 0 - current entity is taking another turn.
        /// </summary>
        public int TurnOffset { get; private set; }

        public List<GameEntity> TurnOrder { get; private set; }
        public GameEntity Entity_OfCurrentTurn => TurnOrder[TurnIndex];
        public int Entity_OfCurrentTurn_Id => Entity_OfCurrentTurn.Scene_GameObject_ID;
        
        public GameEntity[] Players => GameWorld.PlayerRoster.Entities;
        public GameEntity[] ConsciousPlayers { get { List<GameEntity> cp = new List<GameEntity>(); foreach (GameEntity player in Players) if (!player.IsIncapacitated) cp.Add(player); return cp.ToArray(); } }
        public GameEntity[] Enemies => GameWorld.EnemyRoster.Entities;

        internal GameEntity Get_Target(int index)
        {
            if (index >= GameState_Machine.MAX_TEAM_SIZE)
                return Enemies[index % GameState_Machine.MAX_TEAM_SIZE];
            return Players[index];
        }

        public Combat_GameState()
        {
            TurnOffset = 1;
        }

        protected override void Handle_AcquiredWorld()
        {
            GameWorld.Server.ServerSide_Local_Reciever.Register_Handler(
                new MMH_Set_Entity(GameWorld),
                new MMH_Set_Entity_Ready(GameWorld),
                new MMH_Set_Combat_Action(this),
                new MMH_Request_EndTurn(GameWorld)
                );
        }

        private void DictateTurnOrder()
        {
            TurnOrder = new List<GameEntity>();

            GameEntity[] players = Players;
            GameEntity[] enemies = Enemies;

            for(int i=0;i< players.Length;i++)
            {
                TurnOrder.Add(players[i]);
                players[i].Scene_GameObject_ID = i;
                players[i].Initative_Position = i;
            }
            for(int i=0;i<enemies.Length;i++)
            {
                TurnOrder.Add(enemies[i]);
                enemies[i].Scene_GameObject_ID = GameState_Machine.MAX_TEAM_SIZE + i;
                enemies[i].Initative_Position = GameState_Machine.MAX_TEAM_SIZE + i;
            }
        }
        
        protected override void Handle_Update_State(GameState_Machine gameWorld, double deltaTime)
        {
            switch(CombatState)
            {
                case CombatState.BeginNextTurn:
                    BeginTurn();
                    break;
                case CombatState.PlayCurrentTurn:
                    if (Is_EnemyTeam_Incapacitated())
                    {
                        GameWorld.Request_Transition_ToState<Traveling_GameState>();
                        break;
                    }
                    if (Is_AllyTeam_Incapacitated())
                    {
                        GameWorld.Request_Transition_ToState<GameOver_GameState>();
                        break;
                    }
                    Combat_Action action = Entity_OfCurrentTurn.EntityController.Get_CombatAction(this);
                    if (action != null)
                    {
                        if (action.Conduct_Action(this))
                        {
                            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                                new MMW_Announcement(action.CombatAction_Ability_Name)
                                );
                        }
                        else
                        {
                            //TODO: implement.
                        }
                    }
                    break;
                case CombatState.FinishCurrentTurn:
                    Entity_OfCurrentTurn.Combat_EndTurn(this);
                    Progress_TurnOrder();
                    CombatState = CombatState.BeginNextTurn;
                    break;
                case CombatState.FinishCombat:
                    GameWorld.Request_Transition_ToState<Traveling_GameState>();
                    break;
                case CombatState.OutOfCombat:
                default: //do nothing.
                    break;
            }
        }

        private void BeginTurn()
        {
            CombatState = CombatState.PlayCurrentTurn;
            Entity_OfCurrentTurn.Combat_BeginTurn(this);

            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                new MMW_Begin_Turn(Entity_OfCurrentTurn.Scene_GameObject_ID)
                );
        }

        internal void Act_Melee_Attack(int scene_GameObject_ID1, int scene_GameObject_ID2)
        {
            int ally, enemy;
            if (scene_GameObject_ID1 < GameState_Machine.MAX_TEAM_SIZE)
            {
                ally = scene_GameObject_ID1;
                enemy = scene_GameObject_ID2;
            }
            else
            {
                ally = scene_GameObject_ID2;
                enemy = scene_GameObject_ID1;
            }
            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                new MMW_Set_Melee_Combattants(
                    ally,
                    enemy
                    )
                );

            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                new MMW_Invoke_UI_Event(
                    MD_VANILLA_UI.UI_EVENT_MELEE
                    )
                );
        }

        internal void Act_Ranged_Attack(int shooterId, int targetId, string particleType)
        {
            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                new MMW_Set_Ranged_Particle(
                    shooterId,
                    targetId,
                    particleType
                    )
                );
        }

        public void TakeAnExtraTurn() => TurnOffset = 0;
        private void Normalize_TurnProgression() => TurnOffset = 1;
        private void Progress_TurnOrder()
        {
            TurnIndex = (TurnIndex + TurnOffset) % TurnOrder.Count;
            Normalize_TurnProgression();
        }

        public void Request_EndOfTurn()
        {
            CombatState = CombatState.FinishCurrentTurn;
        }

        protected override void Handle_Begin_State(GameState_Machine gameWorld)
        {
            List<GameEntity> enemies = GenerateNewEnemies(gameWorld);
            string[] enemyRaces = new string[enemies.Count];
            for (int i = 0; i < enemies.Count; i++)
                enemyRaces[i] = enemies[i].Race;

            GameWorld.Server.ServerSide_Local_Reciever.Queue_Message(
                new MMW_Set_Party_UI_Descriptions(1, enemyRaces)
                );

            gameWorld.Set_Enemy_Roster(enemies.ToArray());

            gameWorld.Relay_Roster(gameWorld.EnemyRoster);

            CombatState = CombatState.BeginNextTurn;

            DictateTurnOrder();
        }

        protected override void Handle_End_State(GameState_Machine gameWorld)
        {
            gameWorld.Dismiss_Roster(gameWorld.EnemyRoster);
        }

        internal bool Is_AllyTeam_Incapacitated()
        {
            bool ret = true;
            foreach (GameEntity player in Players)
                ret = ret && player.IsIncapacitated;
            return ret;
        }

        internal bool Is_EnemyTeam_Incapacitated()
        {
            bool ret = true;
            foreach (GameEntity enemy in Enemies)
                ret = ret && enemy.IsIncapacitated;
            return ret;
        }

        //TODO: FIX THIS
        private List<GameEntity> GenerateNewEnemies(GameState_Machine gameState)
        {
            return new List<GameEntity>()
            {
                new EC_Goblin(1) { Scene_GameObject_ID = 4 },
                new EC_Goblin(1) { Scene_GameObject_ID = 5 },
                new EC_Goblin(1) { Scene_GameObject_ID = 6 },
                new EC_Goblin(1) { Scene_GameObject_ID = 7 }
            };
        }
    }
}
