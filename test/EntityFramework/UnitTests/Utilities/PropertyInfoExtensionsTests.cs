namespace System.Data.Entity.Utilities
{
    using System.Collections.Generic;
    using System.Data.Entity.Edm;
    using System.Data.Entity.Spatial;
    using System.IO;
    using System.Reflection;
    using Xunit;

    public sealed class PropertyInfoExtensionsTests
    {
        [Fact]
        public void IsValidStructuralProperty_should_return_true_when_property_read_write()
        {
            var mockProperty = new MockPropertyInfo(typeof(int), "P");

            Assert.True(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidStructuralProperty_should_return_false_when_property_invalid()
        {
            var mockProperty = new MockPropertyInfo(typeof(object), "P");

            Assert.False(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidStructuralProperty_should_return_false_when_property_abstract()
        {
            var mockProperty = new MockPropertyInfo().Abstract();

            Assert.False(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidStructuralProperty_should_return_false_when_property_write_only()
        {
            var mockProperty = new MockPropertyInfo();
            mockProperty.SetupGet(p => p.CanRead).Returns(false);

            Assert.False(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidStructuralProperty_should_return_false_when_property_read_only()
        {
            var mockProperty = new MockPropertyInfo();
            mockProperty.SetupGet(p => p.CanWrite).Returns(false);

            Assert.False(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidStructuralProperty_should_return_true_when_property_read_only_collection()
        {
            var mockProperty = new MockPropertyInfo(typeof(List<string>), "P");
            mockProperty.SetupGet(p => p.CanWrite).Returns(false);

            Assert.True(mockProperty.Object.IsValidStructuralProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_nullable_scalar()
        {
            var mockProperty = new MockPropertyInfo(typeof(int?), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_string()
        {
            var mockProperty = new MockPropertyInfo(typeof(string), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_byte_array()
        {
            var mockProperty = new MockPropertyInfo(typeof(byte[]), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }


        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_geography()
        {
            var mockProperty = new MockPropertyInfo(typeof(DbGeography), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_geometry()
        {
            var mockProperty = new MockPropertyInfo(typeof(DbGeometry), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_scalar()
        {
            var mockProperty = new MockPropertyInfo(typeof(decimal), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_enum()
        {
            var mockProperty = new MockPropertyInfo(typeof(FileMode), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_true_for_nullable_enum()
        {
            var mockProperty = new MockPropertyInfo(typeof(FileMode?), "P");

            Assert.True(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void IsValidEdmScalarProperty_should_return_false_when_invalid_type()
        {
            var mockProperty = new MockPropertyInfo(typeof(object), "P");

            Assert.False(mockProperty.Object.IsValidEdmScalarProperty());
        }

        [Fact]
        public void AsEdmPrimitiveProperty_sets_fields_from_propertyInfo()
        {
            var propertyInfo = typeof(PropertyInfoExtensions_properties_fixture).GetProperty("Key");
            var property = propertyInfo.AsEdmPrimitiveProperty();

            Assert.Equal("Key", property.Name);
            Assert.Equal(false, property.PropertyType.IsNullable);
            Assert.Equal(EdmPrimitiveType.Int32, property.PropertyType.PrimitiveType);
        }

        [Fact]
        public void AsEdmPrimitiveProperty_sets_is_nullable_for_nullable_type()
        {
            PropertyInfo propertyInfo = new MockPropertyInfo(typeof(string), null);
            var property = propertyInfo.AsEdmPrimitiveProperty();

            Assert.Equal(true, property.PropertyType.IsNullable);
        }

        [Fact]
        public void AsEdmPrimitiveProperty_returns_null_for_non_primitive_type()
        {
            var propertyInfo = typeof(PropertyInfoExtensions_properties_fixture).GetProperty("EdmProperty");
            var property = propertyInfo.AsEdmPrimitiveProperty();

            Assert.Null(property);
        }

        private class PropertyInfoExtensions_properties_fixture
        {
            public int Key { get; set; }
            public EdmEntityType EdmProperty { get; set; }
        }
    }
}