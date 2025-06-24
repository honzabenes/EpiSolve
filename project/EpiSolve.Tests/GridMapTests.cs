using NUnit.Framework;
using EpiSolve;

namespace EpiSolve.Tests
{
    class GridMapTests
    {
        [Test]
        public void Constructor_InitializesCorrectDimensionsAndTiles()
        {
            int height = 5;
            int width = 7;

            var grid = new GridMap(height, width);

            Assert.AreEqual(height, grid.Height);
            Assert.AreEqual(width, grid.Width);
            Assert.IsNotNull(grid.Tiles);
            Assert.AreEqual(height, grid.Tiles.GetLength(0)); // Počet řádků odpovídá výšce
            Assert.AreEqual(width, grid.Tiles.GetLength(1));  // Počet sloupců odpovídá šířce

            // Všechna pole nastavena na Safe
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Assert.AreEqual(TileState.Safe, grid.Tiles[y, x]);
                }
            }
        }


        [Test]
        public void IsValidPosition_ReturnsTrueForValidCoordinates()
        {
            // Arrange
            var grid = new GridMap(10, 10);

            // Act & Assert
            Assert.IsTrue(grid.isValidPosition(0, 0), "Top-left corner should be valid.");
            Assert.IsTrue(grid.isValidPosition(9, 9), "Bottom-right corner should be valid.");
            Assert.IsTrue(grid.isValidPosition(5, 5), "Center should be valid.");
            Assert.IsTrue(grid.isValidPosition(0, 5), "Top edge should be valid.");
            Assert.IsTrue(grid.isValidPosition(9, 5), "Bottom edge should be valid.");
            Assert.IsTrue(grid.isValidPosition(5, 0), "Left edge should be valid.");
            Assert.IsTrue(grid.isValidPosition(5, 9), "Right edge should be valid.");
        }


        [Test]
        public void IsValidPosition_ReturnsFalseForInvalidCoordinates()
        {
            // Arrange
            var grid = new GridMap(10, 10);

            // Act & Assert
            Assert.IsFalse(grid.isValidPosition(-1, 5), "Negative Y should be invalid.");
            Assert.IsFalse(grid.isValidPosition(5, -1), "Negative X should be invalid.");
            Assert.IsFalse(grid.isValidPosition(10, 5), "Y equal to Height should be invalid.");
            Assert.IsFalse(grid.isValidPosition(5, 10), "X equal to Width should be invalid.");
            Assert.IsFalse(grid.isValidPosition(-1, -1), "Both negative should be invalid.");
            Assert.IsFalse(grid.isValidPosition(10, 10), "Both equal to dimension should be invalid.");
            Assert.IsFalse(grid.isValidPosition(10, -1), "Invalid Y, invalid X should be invalid.");
            Assert.IsFalse(grid.isValidPosition(-1, 10), "Invalid Y, invalid X should be invalid.");
        }


        [Test]
        public void IsValidPosition_WithGridPosition_ReturnsCorrectResult()
        {
            var grid = new GridMap(10, 10);

            Assert.IsTrue(grid.isValidPosition(new GridPosition(0, 0)), "Top-left corner should be valid (GridPosition).");
            Assert.IsFalse(grid.isValidPosition(new GridPosition(-1, 5)), "Negative Y should be invalid (GridPosition).");
        }
    }
}
