using NUnit.Framework;
using EpiSolve;

namespace EpiSolve.Tests
{
    class AgentTests
    {
        [Test]
        public void AgentConstructor_InitializesCorrectly()
        {
            var initialStatus = SIR.Susceptible;
            var initialAge = AgentAge.Adult;
            var initialPosition = new GridPosition(5, 5);

            var agent = new Agent(initialStatus, initialAge, initialPosition);

            Assert.AreEqual(initialStatus, agent.Status);
            Assert.AreEqual(initialAge, agent.Age);
            Assert.AreEqual(initialPosition, agent.Position);
            Assert.IsFalse(agent.beenInfected);
            Assert.AreEqual(1, agent.TimeInfected);
            Assert.AreEqual(1, agent.TimeRecovered);
        }

        // Testování na kraji mapy
        [Test]
        public void GetTilesInRange_ReturnsCorrectTiles_NearEdge()
        {
            // Arrange
            var grid = new GridMap(10, 10);
            var agent = new Agent(SIR.Susceptible, AgentAge.Adult, new GridPosition(0, 0)); // Agent v levém horním rohu
            var movesRanges = new int[] { -1, 0, 1 };

            // Možné pohyby z (0,0) s range {-1, 0, 1}:
            // (-1,-1), (-1,0), (-1,1) - invalidní
            // (0,-1), (0,0), (0,1) - (0,0), (0,1) validní
            // (1,-1), (1,0), (1,1) - (1,0), (1,1) validní
            var expectedPositions = new List<GridPosition>
            {
                new GridPosition(0, 0), new GridPosition(0, 1),
                new GridPosition(1, 0), new GridPosition(1, 1)
            };

            // Act
            var possibleMoves = agent.GetTilesInRange(grid, movesRanges);

            // Assert
            Assert.AreEqual(expectedPositions.Count, possibleMoves.Count);
            foreach (var expectedPos in expectedPositions)
            {
                Assert.Contains(expectedPos, possibleMoves);
            }
        }
    }
}
