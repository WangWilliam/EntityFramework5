namespace System.Data.Entity.ModelConfiguration.Conventions.UnitTests
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration.Configuration.Types;
    using Xunit;

    public sealed class TableAttributeConventionTests
    {
        [Fact]
        public void Apply_should_set_table_name()
        {
            var entityTypeConfiguration = new EntityTypeConfiguration(typeof(object));

            new TableAttributeConvention.TableAttributeConventionImpl()
                .Apply(new MockType(), entityTypeConfiguration, new TableAttribute("Foo"));

            Assert.Equal("Foo", entityTypeConfiguration.GetTableName().Name);
        }

        [Fact]
        public void Apply_should_not_set_table_name_when_already_set()
        {
            var entityTypeConfiguration = new EntityTypeConfiguration(typeof(object));
            entityTypeConfiguration.ToTable("Bar");

            new TableAttributeConvention.TableAttributeConventionImpl()
                .Apply(new MockType(), entityTypeConfiguration, new TableAttribute("Foo"));

            Assert.Equal("Bar", entityTypeConfiguration.GetTableName().Name);
        }

        [Fact]
        public void Apply_should_set_schema_name()
        {
            var entityTypeConfiguration = new EntityTypeConfiguration(typeof(object));

            new TableAttributeConvention.TableAttributeConventionImpl()
                .Apply(new MockType(), entityTypeConfiguration, new TableAttribute("Foo") { Schema = "Bar" });

            Assert.Equal("Bar", entityTypeConfiguration.GetTableName().Schema);
        }
    }
}