namespace RogueCore
{
    namespace Helpers
    {
        public static class GameSettings
        {
            public static class Dungeon
            {
                public const int MapWidth = 500;
                public const int MapHeight = 500;
        
                public const int MinRoomWidth = 100;
                public const int MinRoomHeight = 100;
                
                public const int MaxRoomNum = 20;
                
                public const int MaxRoomWidth = 200;
                public const int MaxRoomHeight = 200;

                public const float MinShrink = 0.5f;
                public const float MaxShrink = 0.8f;
            }
        }
    }
}
//data like this might be better as JSON but this fine now
