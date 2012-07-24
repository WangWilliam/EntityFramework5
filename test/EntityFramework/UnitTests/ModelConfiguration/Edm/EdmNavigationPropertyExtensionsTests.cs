namespace System.Data.Entity.ModelConfiguration.Edm.UnitTests
{
    using System.Data.Entity.Edm;
    using Xunit;

    public sealed class EdmNavigationPropertyExtensionsTests
    {
        [Fact]
        public void Can_get_and_set_configuration_facet()
        {
            var navigationProperty = new EdmNavigationProperty();
            navigationProperty.SetConfiguration(42);

            Assert.Equal(42, navigationProperty.GetConfiguration());
        }
    }
}