﻿using MonkeyDungeon_Core.GameFeatures;
using MonkeyDungeon_Core.GameFeatures.GameStates;
using MonkeyDungeon_Vanilla_Domain.Multiplayer; 

namespace MonkeyDungeon_Core
{
    public class MonkeyDungeon_Server : Multiplayer_Relay_Manager
    {
        public GameState_Machine GameState_Machine { get; private set; }

        public MonkeyDungeon_Server(
            Multiplayer_Relay localRelay
            )
            :base(localRelay)
        {
            GameState_Machine = new GameState_Machine(
                this,
                new GameState[]
                {
                    new Traveling_GameState(),
                    new Combat_GameState(),
                    new Shopping_GameState(),
                    new GameOver_GameState()
                }
                );
        }

        public void On_Update_Frame(double deltaTime)
        {
            Check_Relays();

            GameState_Machine.CheckFor_GameState_Transition(deltaTime);

            Flush_Relays();
        }
    }
}