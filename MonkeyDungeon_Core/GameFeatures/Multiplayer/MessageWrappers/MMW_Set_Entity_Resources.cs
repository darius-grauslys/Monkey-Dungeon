﻿using MonkeyDungeon_Vanilla_Domain.Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyDungeon_Core.GameFeatures.Multiplayer.MessageWrappers
{
    public class MMW_Set_MD_VANILLA_RESOURCES : Multiplayer_Message_Wrapper
    {
        public MMW_Set_MD_VANILLA_RESOURCES(int entityId, params string[] resourceNames) 
            : base(MD_VANILLA_MMH.MMH_SET_MD_VANILLA_RESOURCES, entityId, 0, 0, String.Join(" ", resourceNames))
        {
        }
    }
}