using NUnit.Framework;

namespace ServiceWorker.Test
{
    [TestFixture]
    public class ServiceWorkerTests
    {
        [Test]
        public void TestMethod()
        {
            // Arrange
            int expected = 5;
            int actual = 2 + 3;

            // Act & Assert
            Assert.AreEqual(expected, actual, "The sum of 2 and 3 should be 5");
        }
    }
}