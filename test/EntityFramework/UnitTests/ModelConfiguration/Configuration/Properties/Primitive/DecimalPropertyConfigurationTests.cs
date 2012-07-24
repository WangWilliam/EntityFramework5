namespace System.Data.Entity.ModelConfiguration.Configuration.UnitTests
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Edm;
    using System.Data.Entity.ModelConfiguration.Configuration.Properties.Primitive;
    using System.Data.Entity.ModelConfiguration.Edm;
    using Xunit;
    using Strings = System.Data.Entity.Resources.Strings;

    public sealed class DecimalPropertyConfigurationTests : PrimitivePropertyConfigurationTests
    {
        [Fact]
        public void Configure_should_update_model_decimal_property_precision()
        {
            var configuration = CreateConfiguration();
            configuration.Precision = 8;
            var property = new EdmProperty().AsPrimitive();

            configuration.Configure(property);

            Assert.Equal((byte)8, property.PropertyType.PrimitiveTypeFacets.Precision);
            Assert.Equal(null, property.PropertyType.PrimitiveTypeFacets.Scale);
        }

        [Fact]
        public void Configure_should_update_model_decimal_scale_but_not_precision()
        {
            var configuration = CreateConfiguration();
            configuration.Scale = 70;
            var property = new EdmProperty().AsPrimitive();

            configuration.Configure(property);

            Assert.Equal((byte)70, property.PropertyType.PrimitiveTypeFacets.Scale);
            Assert.Equal(null, property.PropertyType.PrimitiveTypeFacets.Precision);
        }

        [Fact]
        public void CopyFrom_overwrites_null_Precision()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.CopyFrom(configurationB);

            Assert.Equal((byte)255, configurationA.Precision);
        }

        [Fact]
        public void CopyFrom_overwrites_non_null_Precision()
        {
            var configurationA = CreateConfiguration();
            configurationA.Precision = 16;
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.CopyFrom(configurationB);

            Assert.Equal((byte)255, configurationA.Precision);
        }

        [Fact]
        public void CopyFrom_overwrites_null_Scale()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.CopyFrom(configurationB);

            Assert.Equal((byte)255, configurationA.Scale);
        }

        [Fact]
        public void CopyFrom_overwrites_non_null_Scale()
        {
            var configurationA = CreateConfiguration();
            configurationA.Scale = 16;
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.CopyFrom(configurationB);

            Assert.Equal((byte)255, configurationA.Scale);
        }

        [Fact]
        public void FillFrom_overwrites_null_Precision()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.FillFrom(configurationB, inCSpace: false);

            Assert.Equal((byte)255, configurationA.Precision);
        }

        [Fact]
        public void FillFrom_does_not_overwrite_non_null_Precision()
        {
            var configurationA = CreateConfiguration();
            configurationA.Precision = 16;
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.FillFrom(configurationB, inCSpace: false);

            Assert.Equal((byte)16, configurationA.Precision);
        }

        [Fact]
        public void FillFrom_overwrites_null_Precision_in_CSpace()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.FillFrom(configurationB, inCSpace: true);

            Assert.Equal((byte)255, configurationA.Precision);
        }

        [Fact]
        public void FillFrom_does_not_overwrite_non_null_Precision_in_CSpace()
        {
            var configurationA = CreateConfiguration();
            configurationA.Precision = 16;
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            configurationA.FillFrom(configurationB, inCSpace: true);

            Assert.Equal((byte)16, configurationA.Precision);
        }

        [Fact]
        public void FillFrom_overwrites_null_Scale()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.FillFrom(configurationB, inCSpace: false);

            Assert.Equal((byte)255, configurationA.Scale);
        }

        [Fact]
        public void FillFrom_does_not_overwrite_non_null_Scale()
        {
            var configurationA = CreateConfiguration();
            configurationA.Scale = 16;
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.FillFrom(configurationB, inCSpace: false);

            Assert.Equal((byte)16, configurationA.Scale);
        }

        [Fact]
        public void FillFrom_overwrites_null_Scale_in_CSpace()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.FillFrom(configurationB, inCSpace: true);

            Assert.Equal((byte)255, configurationA.Scale);
        }

        [Fact]
        public void FillFrom_does_not_overwrite_non_null_Scale_in_CSpace()
        {
            var configurationA = CreateConfiguration();
            configurationA.Scale = 16;
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            configurationA.FillFrom(configurationB, inCSpace: true);

            Assert.Equal((byte)16, configurationA.Scale);
        }

        [Fact]
        public void IsCompatible_returns_errors_for_all_mismatched_decimal_properties()
        {
            var configurationA = CreateConfiguration();
            configurationA.ColumnName = "bar";
            configurationA.ColumnType = "bar";
            configurationA.ColumnOrder = 1;
            configurationA.IsNullable = true;
            configurationA.ConcurrencyMode = EdmConcurrencyMode.None;
            configurationA.DatabaseGeneratedOption = DatabaseGeneratedOption.Computed;
            configurationA.Precision = 16;
            configurationA.Scale = 16;

            var configurationB = CreateConfiguration();
            configurationB.ColumnName = "foo";
            configurationB.ColumnType = "foo";
            configurationB.ColumnOrder = 2;
            configurationB.IsNullable = false;
            configurationB.ConcurrencyMode = EdmConcurrencyMode.Fixed;
            configurationB.DatabaseGeneratedOption = DatabaseGeneratedOption.Identity;
            configurationB.Precision = 255;
            configurationB.Scale = 255;

            var expectedMessageCSpace = Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "IsNullable", true, "IsNullable", false);

            expectedMessageCSpace += Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "ConcurrencyMode", EdmConcurrencyMode.None, "ConcurrencyMode", EdmConcurrencyMode.Fixed);

            expectedMessageCSpace += Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "DatabaseGeneratedOption", DatabaseGeneratedOption.Computed, "DatabaseGeneratedOption", DatabaseGeneratedOption.Identity);

            var expectedMessage = Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "ColumnName", "bar", "ColumnName", "foo");

            expectedMessage += Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "ColumnOrder", 1, "ColumnOrder", 2);

            expectedMessage += Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "ColumnType", "bar", "ColumnType", "foo");

            var additionalErrors = Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "Precision", 16, "Precision", 255);

            additionalErrors += Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "Scale", 16, "Scale", 255);

            expectedMessageCSpace += additionalErrors;
            expectedMessage += additionalErrors;

            string errorMessage;
            Assert.False(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.Equal(expectedMessage, errorMessage);
            Assert.False(configurationA.IsCompatible(configurationB, true, out errorMessage));
            Assert.Equal(expectedMessageCSpace, errorMessage);
        }

        [Fact]
        public void IsCompatible_returns_true_for_matching_Precision()
        {
            var configurationA = CreateConfiguration();
            configurationA.Precision = 255;
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            string errorMessage;
            Assert.True(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));
        }

        [Fact]
        public void IsCompatible_returns_false_for_mismatched_Precision()
        {
            var configurationA = CreateConfiguration();
            configurationA.Precision = 16;
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            var expectedMessage = Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "Precision", 16, "Precision", 255);

            string errorMessage;
            Assert.False(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.Equal(expectedMessage, errorMessage);
            Assert.False(configurationA.IsCompatible(configurationB, true, out errorMessage));
            Assert.Equal(expectedMessage, errorMessage);
        }

        [Fact]
        public void IsCompatible_returns_true_for_null_Precision()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Precision = 255;

            string errorMessage;
            Assert.True(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));

            Assert.True(configurationB.IsCompatible(configurationA, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));
        }

        [Fact]
        public void IsCompatible_returns_true_for_matching_Scale()
        {
            var configurationA = CreateConfiguration();
            configurationA.Scale = 255;
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            string errorMessage;
            Assert.True(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));
        }

        [Fact]
        public void IsCompatible_returns_false_for_mismatched_Scale()
        {
            var configurationA = CreateConfiguration();
            configurationA.Scale = 16;
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            var expectedMessage = Environment.NewLine + "\t" +
                Strings.ConflictingConfigurationValue(
                    "Scale", 16, "Scale", 255);

            string errorMessage;
            Assert.False(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.Equal(expectedMessage, errorMessage);
            Assert.False(configurationA.IsCompatible(configurationB, true, out errorMessage));
            Assert.Equal(expectedMessage, errorMessage);
        }

        [Fact]
        public void IsCompatible_returns_true_for_null_Scale()
        {
            var configurationA = CreateConfiguration();
            var configurationB = CreateConfiguration();
            configurationB.Scale = 255;

            string errorMessage;
            Assert.True(configurationA.IsCompatible(configurationB, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));

            Assert.True(configurationB.IsCompatible(configurationA, false, out errorMessage));
            Assert.True(string.IsNullOrEmpty(errorMessage));
        }

        internal new DecimalPropertyConfiguration CreateConfiguration()
        {
            return (DecimalPropertyConfiguration)base.CreateConfiguration();
        }

        internal override Type GetConfigurationType()
        {
            return typeof(DecimalPropertyConfiguration);
        }
    }
}