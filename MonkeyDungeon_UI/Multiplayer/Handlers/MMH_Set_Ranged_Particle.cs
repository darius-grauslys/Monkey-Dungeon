﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isometricgame.GameEngine.Scenes;
using MonkeyDungeon_UI.Scenes.GameScenes;
using MonkeyDungeon_Vanilla_Domain.Multiplayer;
using OpenTK;

namespace MonkeyDungeon_UI.Multiplayer.Handlers
{
    public class MMH_Set_Ranged_Particle : Multiplayer_Message_UI_Handler
    {
        private World_Layer World_Layer { get; set; }

        public MMH_Set_Ranged_Particle(World_Layer sceneLayer) 
            : base(sceneLayer, MD_VANILLA_MMH.MMH_SET_RANGED_PARTICLE)
        {
            World_Layer = sceneLayer;
        }

        protected override void Handle_Message(Multiplayer_Message recievedMessage)
        {
            int shootingEntity = recievedMessage.ENTITY_ID;
            int shootingTarget = recievedMessage.INT_VALUE;
            string particleType = recievedMessage.STRING_VALUE;

            Vector3 shooterPos = World_Layer.Get_Position_From_Id(shootingEntity);
            Vector3 targetPos = World_Layer.Get_Position_From_Id(shootingTarget);

            World_Layer.Ranged_Particle.Set_Particle(particleType);
            World_Layer.UI_Ranged_Particle_Event.Inital_Position = shooterPos;
            World_Layer.UI_Ranged_Particle_Event.Target_Position = targetPos;

            World_Layer.EventScheduler.Invoke_Event(MD_VANILLA_UI.UI_EVENT_RANGED_ATTACK);
        }
    }
}