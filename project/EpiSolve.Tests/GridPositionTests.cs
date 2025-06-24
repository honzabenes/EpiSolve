using NUnit.Framework;
using EpiSolve;

namespace EpiSolve.Tests
{
    [TestFixture]
    class GridPositionTests
    {
        [Test]
        public void Constructor_InitializesCorrectly()
        {
            int y = 10;
            int x = 20;

            var pos = new GridPosition(y, x);

            Assert.AreEqual(y, pos.Y);
            Assert.AreEqual(x, pos.X);
        }


        [Test]
        public void Equals_ReturnsTrueForSamePosition()
        {
            var pos1 = new GridPosition(5, 5);
            var pos2 = new GridPosition(5, 5);

            Assert.IsTrue(pos1.Equals(pos2));
            Assert.IsTrue(pos1 == pos2);
        }


        [Test]
        public void Equals_ReturnsFalseForDifferentPosition()
        {
            var pos1 = new GridPosition(5, 5);
            var pos2 = new GridPosition(5, 6);
            var pos3 = new GridPosition(6, 5);
            var pos4 = new GridPosition(6, 6);
            var pos5 = new GridPosition(5, 5);


            Assert.IsFalse(pos1.Equals(pos2));
            Assert.IsFalse(pos1 == pos2);
            Assert.IsFalse(pos1.Equals(pos3));
            Assert.IsFalse(pos1 == pos3);
            Assert.IsFalse(pos1.Equals(pos4));
            Assert.IsFalse(pos1 == pos4);
            // Test proti jinému typu objektu
            Assert.IsFalse(pos1.Equals(new object()));
            // Test proti null
            Assert.IsFalse(pos1.Equals(null));
            Assert.IsTrue(pos1.Equals(pos5)); // Mělo by být true, Equals porovnává hodnoty X, Y
            Assert.IsTrue(pos1 == pos5); // Mělo by být true díky přetížení operátoru ==
        }


        [Test]
        public void NotEquals_ReturnsCorrectResult()
        {
            // Arrange
            var pos1 = new GridPosition(5, 5);
            var pos2 = new GridPosition(5, 5);
            var pos3 = new GridPosition(5, 6);

            // Act & Assert
            Assert.IsFalse(pos1 != pos2);
            Assert.IsTrue(pos1 != pos3);
        }
    }
}
