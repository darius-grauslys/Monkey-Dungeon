﻿using System;

namespace MonkeyDungeon_Vanilla_Domain.GameFeatures
{
    public class GameEntity_Survey_Quantity<T, Y> : GameEntity_Survey<Y> where T : GameEntity where Y : GameEntity_Quantity<T>
    {
        private readonly Func<Y> DEFAULT_QUANTITIZER;
        
        public GameEntity_Survey_Quantity(Func<Y> defaultQuantitizer) 
            : base(null)
        {
            DEFAULT_QUANTITIZER = defaultQuantitizer;
            
            GameEntity_Position.For_Each__Position(GameEntity_Team_ID.ID_NULL, (p) =>
            {
                FIELD[p] = defaultQuantitizer();
            });
        }
        
        protected override void Handle__Reset__Survey()
        {
            GameEntity_Position.For_Each__Position(GameEntity_Team_ID.ID_NULL, (p) =>
            {
                FIELD[p] = DEFAULT_QUANTITIZER();
            });
        }
        
        protected override bool Check_If__Equivalent_To_Default__Survey(Y value)
        {
            return value == DEFAULT_VALUE || value == 0;
        }
    }
}