﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyDungeon_Vanilla_Domain.GameFeatures;

namespace MonkeyDungeon_Vanilla_Domain.Multiplayer
{
    /// <summary>
    /// Used by server endpoints to handle multiple relays at once.
    /// </summary>
    public class Multiplayer_Relay_Manager
    {
        private readonly Dictionary<Multiplayer_Relay_ID, Multiplayer_Relay> RELAYS = new Dictionary<Multiplayer_Relay_ID, Multiplayer_Relay>();
        public Multiplayer_Relay Get_Relay(Multiplayer_Relay_ID id)
            => RELAYS[id];
        protected virtual void Add_Relay(Multiplayer_Relay relay)
        {
            relay.Relay_ID = new Multiplayer_Relay_ID(RELAYS.Count);
            RELAYS.Add(relay.Relay_ID, relay);
        }

        /// <summary>
        /// Returns boolean based on bind success.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rid"></param>
        /// <returns></returns>
        public bool Bind_To_Relay(GameEntity_ID id, Multiplayer_Relay_ID rid)
        {
            if (!RELAYS.ContainsKey(rid) || id == GameEntity_ID.ID_NULL)
                return false;

            id.Relay_ID = rid;
            return true;
        }

        /// <summary>
        /// Relay that interfaces to the local machine of the server.
        /// </summary>
        public Multiplayer_Relay Local_Relay { get; protected set; }
        public readonly int LOCAL_RELAY_ID = 0;

        public Multiplayer_Relay_Manager(Multiplayer_Relay localRelay)
        {
            Add_Relay(Local_Relay = localRelay);
        }

        public void Register_Multiplayer_Handlers(params Multiplayer_Message_Handler[] handlers)
        {
            foreach(Multiplayer_Message_Handler handler in handlers)
                foreach (Multiplayer_Relay r in RELAYS.Values)
                    r.Register_Handler(handler);
        }

        public void Broadcast(Multiplayer_Message msg)
        {
            foreach (Multiplayer_Relay r in RELAYS.Values)
                r.Queue_Message(msg);
        }

        public void Relay(Multiplayer_Relay_ID relayID, Multiplayer_Message msg)
            => Get_Relay(relayID).Queue_Message(msg);

        protected void Check_Relays()
        {
            foreach (Multiplayer_Relay r in RELAYS.Values)
                r.CheckFor_NewMessages();
        }

        protected void Flush_Relays()
        {
            foreach (Multiplayer_Relay r in RELAYS.Values)
                r.Flush_Messages();
        }
    }
}
