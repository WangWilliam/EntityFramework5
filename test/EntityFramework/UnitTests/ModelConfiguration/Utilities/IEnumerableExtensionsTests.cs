namespace System.Data.Entity.ModelConfiguration.Utilities.UnitTests
{
    using Xunit;

    public sealed class IEnumerableExtensionsTests
    {
        [Fact]
        public void Each_should_iterate_sequence()
        {
            var i = 0;

            new[] { 1, 2, 3 }.Each(_ => i++);

            Assert.Equal(3, i);
        }
    }
}