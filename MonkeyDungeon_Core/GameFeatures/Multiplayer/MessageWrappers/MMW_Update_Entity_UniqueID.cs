﻿using MonkeyDungeon_Vanilla_Domain.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyDungeon_Core.GameFeatures.Multiplayer.MessageWrappers
{
    public class MMW_Update_Entity_UniqueID : Multiplayer_Message_Wrapper
    {
        public MMW_Update_Entity_UniqueID(int entityId, uint uid) 
            : base(MD_VANILLA_MMH.MMH_UPDATE_ENTITY_UNIQUEID, entityId, 0, (int)uid)
        {
        }
    }
}