﻿using isometricgame.GameEngine;
using isometricgame.GameEngine.Components.Rendering;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Rendering;
using isometricgame.GameEngine.Rendering.Animation;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Rendering;
using MonkeyDungeon_UI.Prefabs.UI;
using MonkeyDungeon_UI.Prefabs.UI.EntityData;
using MonkeyDungeon_Vanilla_Domain.GameFeatures;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyDungeon_UI.Prefabs.Entities
{
    public class CreatureGameObject : GameObject
    {
        public static readonly string Suffix_Unique = "Unique", Suffix_Body = "Body", Suffix_Head = "Head";

        public string Name { get; internal set; }
        public string Race { get; internal set; }
        public int Scene_ID { get; internal set; }

        protected RenderUnit UniqueIdentifier;
        internal uint UniqueIdentifier_TypeIndex { get => UniqueIdentifier.VAO_Index; set { UniqueIdentifier.VAO_Index = value; Has_UniqueIdentifier = true; } }
        public bool Has_UniqueIdentifier { get; private set; }
        protected RenderUnit Body;

        private ResourceBar healthBar;

        private UI_GameEntity_Descriptor entityDescription;
        internal UI_GameEntity_Descriptor EntityDescription
        {
            get => entityDescription; set
            {
                Change_Entity(value);
            }
        }

        private void Change_Entity(UI_GameEntity_Descriptor entity)
        {
            Unbind_To_Description();
            Bind_To_Description(entity);
            Set_Race(entity);
            AnimationComponent.Play(0);
        }

        public void Bind_To_Description(UI_GameEntity_Descriptor entity)
        {
            entityDescription = entity;
            if (entity == null)
                return;
            EntityDescription.Resource_Added += Resource_Added;
            EntityDescription.Entity_Died += Entity_Died;
            EntityDescription.Entity_Dismissal_State_Changed += Entity_Dismissal_State_Changed;
            Entity_Dismissal_State_Changed(entity);
        }
        
        private void Unbind_To_Description()
        {
            if (entityDescription == null)
                return;
            EntityDescription.Resource_Added -= Resource_Added;
            EntityDescription.Entity_Died -= Entity_Died;
            EntityDescription.Entity_Dismissal_State_Changed -= Entity_Dismissal_State_Changed;
            entityDescription = null;
        }

        private void Resource_Added(UI_GameEntity_Resource resource)
        {
            if (resource.Resource_Name == healthBar.Resource_Name)
            {
                resource.Resource_Removed += (e) => e.Resource_Updated -= Check_Health;
                resource.Resource_Updated += Check_Health;

                healthBar.Attach_To_Resource(resource);
            }
        }

        private void Check_Health(float val)
        {
            if (val > 0.75f)
                AnimationComponent.SetNode(0);
            else if (val > 0.50f)
                AnimationComponent.SetNode(1);
            else if (val > 0.25f)
                AnimationComponent.SetNode(2);
            else if (val > 0)
                AnimationComponent.SetNode(3);
        }

        private void Entity_Died(UI_GameEntity_Descriptor entity)
        {
            AnimationComponent.Play(4);
        }

        private void Entity_Dismissal_State_Changed(UI_GameEntity_Descriptor obj)
        {
            AnimationComponent.Enabled = !obj.IsDismissed;
        }

        public string UI_Race => EntityDescription.RACE;

        private AnimationComponent AnimationComponent;
        public Vector3 Inital_Position { get; private set; }

        public CreatureGameObject(SceneLayer sceneLayer, Vector3 position, UI_GameEntity_Descriptor entity = null, int animRow = 8, int animCol = 8)
            : base(sceneLayer, position)
        {
            Inital_Position = position;
            healthBar = new ResourceBar(
                sceneLayer,
                position + new Vector3(0, -35, 0),
                isometricgame.GameEngine.Systems.MathHelper.Color_To_Vec4(Color.Red),
                MD_VANILLA_RESOURCES.RESOURCE_HEALTH
                );

            //head anim
            AnimationSchematic schem = new AnimationSchematic(8, 0.1);
            AnimationNode[] nodes = new AnimationNode[animRow];
            for (int i = 0; i < animRow; i++)
            {
                int[] subNodes = new int[animCol];
                for (int j = 0; j < animCol; j++)
                    subNodes[j] = i * animCol + j;
                nodes[i] = new AnimationNode(schem, subNodes, -1);
                schem.DefineNode(i, nodes[i]);
            }

            nodes[4].Pauses_OnCompletion = true;
            nodes[4].Speed = 0.2;

            SpriteComponent = AnimationComponent = new AnimationComponent(schem);
            AnimationComponent.SetNode(0);

            EntityDescription = entity;
        }

        internal void Set_Race(UI_GameEntity_Descriptor entity)
        {
            //TODO: Centralize primitives.
            string race = entity?.RACE ?? "Monkey";
            uint uniqueIdentifier = entity?.UNIQUE_IDENTIFIER ?? 0;

            string h = race + Suffix_Head;
            string b = race + Suffix_Body;
            string u = race + Suffix_Unique;

            if (!SceneLayer.Game.SpriteLibrary.HasSprite(h))
                throw new ArgumentException();

            Race = entity?.RACE ?? "Monkey";

            SpriteComponent.SetSprite(h);
            Body = SceneLayer.Game.SpriteLibrary.ExtractRenderUnit(b);

            if (Has_UniqueIdentifier = SceneLayer.Game.SpriteLibrary.HasSprite(u))
            {
                UniqueIdentifier = SceneLayer.Game.SpriteLibrary.ExtractRenderUnit(u);
                UniqueIdentifier.VAO_Index = uniqueIdentifier;
            }
        }

        internal void Set_Unique_ID(uint uid)
        {
            UniqueIdentifier.VAO_Index = uid;
            EntityDescription.UNIQUE_IDENTIFIER = uid;
        }

        protected override void HandleDraw(RenderService renderService)
        {
            float bodyHeight = 1;
            bodyHeight = SceneLayer.Game.SpriteLibrary.GetSprite(Race + Suffix_Body).SubHeight;
            if (SpriteComponent.Enabled)
            {
                renderService.DrawSprite(ref Body, Position.X, Position.Y - (bodyHeight * 0.3f));
                renderService.DrawObj(healthBar);
            }

            base.HandleDraw(renderService);
            if (SpriteComponent.Enabled && Has_UniqueIdentifier)
                renderService.DrawSprite(ref UniqueIdentifier, Position.X, Position.Y + (bodyHeight * 0.3f));
        }
    }
}
