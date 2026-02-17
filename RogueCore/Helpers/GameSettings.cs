namespace RogueCore
{
    namespace Helpers
    {
        public static class GameSettings
        {
            public static class Dungeon
            {
                public const int MapWidth = 100;
                public const int MapHeight = 100;
        
                public const int MinRoomWidth = 10;
                public const int MinRoomHeight = 10;
                
                public const int MaxRoomNum = 5;
                
                public const int MaxRoomWidth = 20;
                public const int MaxRoomHeight = 20;

                public const float MinShrink = 0.5f;
                public const float MaxShrink = 0.8f;
            }

            public static class Interior
            {
                public const int MaxNumOfCoins = 10;
                public const int NumOfCoinsPerRoom = 5;
                public const int MaxNumOfEnemies = 10;
                public const int MaxChasingEnemiesPerRoom = 3;
                public const int MaxPatrollingEnemiesPerRoom = 1;
                public const int MaxObservingEnemiesPerRoom = 3;
                public const int MinFoodPerRoom = 1;
                public const int MaxFoodPerRoom = 5;
            }
            
            public static class Player
            {
                public const int MaxHealth = 100;
            }
        }
    }
}
//data like this might be better as JSON but this fine now
