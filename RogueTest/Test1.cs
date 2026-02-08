using RogueCore.Helpers;

namespace RogueTest;
using RogueCore.Services.Dungeon;
[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        DungeonGenerator dungeonGenerator = new DungeonGenerator();
        dungeonGenerator.GenerateDungeon();
    }
}