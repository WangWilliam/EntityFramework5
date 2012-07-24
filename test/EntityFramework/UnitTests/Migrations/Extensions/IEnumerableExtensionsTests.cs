namespace System.Data.Entity.Migrations
{
    using System.Data.Entity.Migrations.Extensions;
    using Xunit;

    public class IEnumerableExtensionsTests
    {
        [Fact]
        public void Each_should_iterate_sequence()
        {
            var i = 0;

            new[] { 1, 2, 3 }.Each(_ => i++);

            Assert.Equal(3, i);
        }

        [Fact]
        public void Join_should_return_joined_string()
        {
            Assert.Equal("1, 2, 3", new[] { 1, 2, 3 }.Join());
            Assert.Equal("1-2-3", new[] { 1, 2, 3 }.Join(separator: "-"));
            Assert.Equal("s, s, s", new[] { 1, 2, 3 }.Join(i => "s"));
        }
    }
}