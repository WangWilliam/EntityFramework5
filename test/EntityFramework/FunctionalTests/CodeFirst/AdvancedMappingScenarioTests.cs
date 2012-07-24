namespace FunctionalTests
{
    using System;
    using System.Data.Entity.Core;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Entity.Edm;
    using System.Data.Entity.Resources;
    using System.Linq;
    using System.Linq.Expressions;
    using FunctionalTests.Fixtures;
    using FunctionalTests.Model;
    using Xunit;

    namespace Fixtures
    {
        using System.Collections.Generic;
        using System.ComponentModel.DataAnnotations;
        using System.ComponentModel.DataAnnotations.Schema;

        public class StockOrder
        {
            public int Id { get; set; }
            public int LocationId { get; set; }
            public Location Location { get; set; }
            public ICollection<Organization> Organizations { get; set; }
        }

        public class Organization
        {
            public int Id { get; set; }
            public int StockOrderId { get; set; }
            public StockOrder StockOrder { get; set; }
            public ICollection<Location> Locations { get; set; }
        }

        public class Location
        {
            public int Id { get; set; }
            public ICollection<StockOrder> StockOrders { get; set; }
            public int OrganizationId { get; set; }
            public Organization Organization { get; set; }
        }

        public class DecimalKey
        {
            public decimal Id { get; set; }
            public ICollection<DecimalDependent> DecimalDependents { get; set; }
        }

        public class DecimalDependent
        {
            public int Id { get; set; }
            public decimal DecimalKeyId { get; set; }
        }

        public abstract class SingleAbstract
        {
            public int Id { get; set; }
            public DecimalDependent Nav { get; set; }
        }

        public class SplitProduct
        {
            public int Id { get; set; }
            public string Name { get; set; }

            [Required]
            public SplitProductDetail Detail { get; set; }
        }

        public class SplitProductDetail
        {
            [ForeignKey("Product")]
            public int Id { get; set; }

            public string Name { get; set; }

            [Required]
            public SplitProduct Product { get; set; }
        }

        public class EntityWithConfiguredDuplicateColumn
        {
            public int Id { get; set; }
            public string Description { get; set; }

            [Column("Description")]
            public string Details { get; set; }
        }

        public class EntityWithDescBase
        {
            public int Id { get; set; }
        }

        public class EntityWithDescA : EntityWithDescBase
        {
            public string Description { get; set; }
            public ComplexWithDesc Complex { get; set; }
        }

        public class EntityWithDescB : EntityWithDescBase
        {
            public string Description { get; set; }
            public ComplexWithDesc Complex { get; set; }
        }

        public class EntityWithDescC : EntityWithDescBase
        {
            public string Description { get; set; }
            public ComplexWithDesc Complex { get; set; }
        }

        public class ComplexWithDesc
        {
            [Required]
            public string Description { get; set; }
        }

        public class MaxLengthProperties
        {
            public string Id { get; set; }
            public string Prop1 { get; set; }
            public byte[] Prop2 { get; set; }
        }
    }

    public sealed class AdvancedMappingScenarioTests : TestBase
    {
        [Fact]
        public void Sql_ce_should_get_explicit_max_lengths_for_string_and_binary_properties_by_convention()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<MaxLengthProperties>();

            var databaseMapping = BuildCeMapping(modelBuilder);

            databaseMapping.AssertValid();

            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbEqual("nvarchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbEqual("nvarchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbEqual("varbinary", c => c.TypeName);
        }

        [Fact]
        public void Sql_ce_should_get_explicit_max_lengths_for_fixed_length_string_and_fixed_length_binary_properties_by_convention()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Id).IsFixedLength();
            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Prop1).IsFixedLength();
            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Prop2).IsFixedLength();

            var databaseMapping = BuildCeMapping(modelBuilder);

            databaseMapping.AssertValid();

            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbEqual("nchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbEqual("nchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(4000, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbEqual("binary", c => c.TypeName);
        }

        [Fact]
        public void Sql_should_get_implicit_max_lengths_for_string_and_binary_properties_by_convention()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<MaxLengthProperties>();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();

            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(128, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbEqual("nvarchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(null, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbEqual("nvarchar(max)", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(null, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbEqual("varbinary(max)", c => c.TypeName);
        }

        [Fact]
        public void Sql_should_get_explicit_max_lengths_for_fixed_length_string_and_fixed_length_binary_properties_by_convention()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Id).IsFixedLength();
            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Prop1).IsFixedLength();
            modelBuilder.Entity<MaxLengthProperties>().Property(e => e.Prop2).IsFixedLength();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();

            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(128, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Id).DbEqual("nchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(128, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop1).DbEqual("nchar", c => c.TypeName);

            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(128, f => f.MaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbFacetEqual(null, f => f.IsMaxLength);
            databaseMapping.Assert<MaxLengthProperties>(e => e.Prop2).DbEqual("binary", c => c.TypeName);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_column_is_uniquified()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithConfiguredDuplicateColumn>();

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<EntityWithConfiguredDuplicateColumn>(e => e.Description).DbEqual("Description1",
                                                                                                    c => c.Name);
            databaseMapping.Assert<EntityWithConfiguredDuplicateColumn>(e => e.Details).DbEqual("Description",
                                                                                                c => c.Name);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_columns_are_uniquified_first()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescBase>();
            modelBuilder.Entity<EntityWithDescA>().Property(e => e.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<EntityWithDescA>(e => e.Description).DbEqual("Description", c => c.Name);
            databaseMapping.Assert<EntityWithDescB>(e => e.Description).DbEqual("Description1", c => c.Name);
            databaseMapping.Assert<EntityWithDescC>(e => e.Description).DbEqual("Description2", c => c.Name);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_columns_are_uniquified_second()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescBase>();
            modelBuilder.Entity<EntityWithDescB>().Property(e => e.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<EntityWithDescA>(e => e.Description).DbEqual("Description1", c => c.Name);
            databaseMapping.Assert<EntityWithDescB>(e => e.Description).DbEqual("Description", c => c.Name);
            databaseMapping.Assert<EntityWithDescC>(e => e.Description).DbEqual("Description2", c => c.Name);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_columns_are_uniquified_third()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescBase>();
            modelBuilder.Entity<EntityWithDescC>().Property(e => e.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<EntityWithDescA>(e => e.Description).DbEqual("Description1", c => c.Name);
            databaseMapping.Assert<EntityWithDescB>(e => e.Description).DbEqual("Description2", c => c.Name);
            databaseMapping.Assert<EntityWithDescC>(e => e.Description).DbEqual("Description", c => c.Name);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_columns_are_uniquified_complex()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescA>().Property(e => e.Complex.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<ComplexWithDesc>(c => c.Description).DbEqual("Description", c => c.Name);
            databaseMapping.Assert<ComplexWithDesc>(c => c.Description).DbEqual(false, c => c.IsNullable);
            databaseMapping.Assert<EntityWithDescA>(e => e.Description).DbEqual("Description1", c => c.Name);
        }

        [Fact]
        public void Can_have_configured_complex_column_override_column_name_clash()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescA>().Property(e => e.Complex.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<ComplexWithDesc>(c => c.Description).DbEqual(false, c => c.IsNullable);
            databaseMapping.Assert<ComplexWithDesc>(c => c.Description).DbEqual("Description", c => c.Name);
            databaseMapping.Assert<EntityWithDescA>(e => e.Description).DbEqual("Description1", c => c.Name);
        }

        [Fact]
        public void Can_have_configured_duplicate_column_and_by_convention_columns_are_uniquified_conflict()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<EntityWithDescBase>();
            modelBuilder.Entity<EntityWithDescB>().Property(e => e.Description).HasColumnName("Description");
            modelBuilder.Entity<EntityWithDescC>().Property(e => e.Description).HasColumnName("Description");

            var databaseMapping = BuildMapping(modelBuilder);

            Assert.Throws<MetadataException>(() => databaseMapping.AssertValid());
        }

        [Fact]
        public void Can_table_split_and_conflicting_columns_are_uniquified()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<SplitProduct>().ToTable("Product");
            modelBuilder.Entity<SplitProductDetail>().ToTable("Product");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Can_table_split_and_conflicting_columns_can_be_configured()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<SplitProduct>().ToTable("Product");
            modelBuilder.Entity<SplitProductDetail>().ToTable("Product");
            modelBuilder.Entity<SplitProductDetail>().Property(s => s.Name).HasColumnName("Unique");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<SplitProduct>(s => s.Name).DbEqual("Name", c => c.Name);
            databaseMapping.Assert<SplitProductDetail>(s => s.Name).DbEqual("Unique", c => c.Name);
        }

        [Fact]
        public void Single_abstract_type_with_associations_throws_not_mappable_exception()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<SingleAbstract>();

            Assert.Equal(Strings.UnmappedAbstractType(typeof(SingleAbstract)),
                         Assert.Throws<InvalidOperationException>(
                             () => modelBuilder.Build(ProviderRegistry.Sql2008_ProviderInfo)).Message);
        }

        [Fact]
        public void Configured_decimal_key_gets_correct_facet_defaults()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<DecimalKey>().HasKey(d => d.Id);

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
            databaseMapping.Assert<DecimalKey>(d => d.Id).FacetEqual((byte)18, f => f.Precision);
            databaseMapping.Assert<DecimalKey>(d => d.Id).FacetEqual((byte)2, f => f.Scale);
        }

        [Fact]
        public void Decimal_key_with_custom_store_type_should_propagate_facets()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<DecimalKey>().Property(p => p.Id).HasColumnType("money");

            var databaseMapping = BuildMapping(modelBuilder);

            databaseMapping.AssertValid();
        }

        [Fact]
        public void Throw_when_mapping_properties_expression_contains_assignments()
        {
            var modelBuilder = new DbModelBuilder();

            Expression<Func<StockOrder, object>> propertiesExpression = so => new { Foo = so.LocationId };

            Assert.Equal(Strings.InvalidComplexPropertiesExpression(propertiesExpression),
                         Assert.Throws<InvalidOperationException>(() =>
                                                                  modelBuilder
                                                                      .Entity<StockOrder>()
                                                                      .Map(emc => emc.Properties(propertiesExpression)))
                             .Message);
        }

        [Fact]
        public void Circular_delete_cascade_path_can_be_generated()
        {
            var modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<StockOrder>();

            var databaseMapping = BuildMapping(modelBuilder);

            Assert.Equal(3,
                         databaseMapping.Model.Namespaces
                             .Single()
                             .AssociationTypes
                             .SelectMany(a => a.Members)
                             .Cast<EdmAssociationEnd>()
                             .Count(e => e.DeleteAction == EdmOperationAction.Cascade));
        }

        [Fact]
        public void Build_model_for_entity_splitting_difference_schemas()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Vendor>()
                .Map(m =>
                     {
                         m.Properties(v1 => new
                                            {
                                                v1.VendorID,
                                                v1.Name,
                                                v1.PreferredVendorStatus,
                                                v1.AccountNumber,
                                                v1.ActiveFlag,
                                                v1.CreditRating
                                            });
                         m.ToTable("Vendor", "vendors");
                     })
                .Map(m =>
                     {
                         m.Properties(v2 => new { v2.VendorID, v2.ModifiedDate, v2.PurchasingWebServiceURL });
                         m.ToTable("VendorDetails", "details");
                     });

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "vendors"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "details"));
        }

        [Fact]
        public void Build_model_for_mapping_to_duplicate_tables_different_schemas()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<Customer>().ToTable("tbl");
            modelBuilder.Entity<Product>().ToTable("tbl", "other");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "other"));

            databaseMapping.Assert<Customer>().DbEqual("tbl", t => t.DatabaseIdentifier);
            databaseMapping.Assert<Product>().DbEqual("tbl", t => t.DatabaseIdentifier);
        }

        [Fact]
        public void Build_model_after_configuring_entity_set_name()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<TransactionHistoryArchive>().HasEntitySetName("Foos");

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Model.Containers.Single().EntitySets.Any(es => es.Name == "Foos"));
        }

        [Fact]
        public void Build_model_with_dotted_table_name_configured()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().Map(mc => mc.ToTable("sales.tbl_sp"));

            modelBuilder.Entity<Customer>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "sales"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
        }

        [Fact]
        public void Build_model_with_dotted_table_name_and_dotted_schema_configured()
        {
            var modelBuilder = new AdventureWorksModelBuilder();

            modelBuilder.Entity<SalesPerson>().Map(mc => mc.ToTable("sales.A.B.tbl_sp"));
            modelBuilder.Entity<Customer>();

            var databaseMapping = modelBuilder.BuildAndValidate(ProviderRegistry.Sql2008_ProviderInfo);

            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "sales.A.B"));
            Assert.True(databaseMapping.Database.Schemas.Any(s => s.DatabaseIdentifier == "dbo"));
        }
    }
}