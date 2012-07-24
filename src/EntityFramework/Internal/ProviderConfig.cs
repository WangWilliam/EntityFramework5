namespace System.Data.Entity.Internal
{
    using System.Data.Entity.Config;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Internal.ConfigFile;
    using System.Data.Entity.Migrations.Sql;
    using System.Data.Entity.Resources;
    using System.Data.Entity.Utilities;
    using System.Diagnostics.Contracts;
    using System.Linq;

    internal class ProviderConfig
    {
        private readonly EntityFrameworkSection _entityFrameworkSettings;

        public ProviderConfig(EntityFrameworkSection entityFrameworkSettings)
        {
            _entityFrameworkSettings = entityFrameworkSettings;
        }

        public DbProviderServices TryGetDbProviderServices(string providerInvariantName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(providerInvariantName));

            var providerElement = TryGetProviderElement(providerInvariantName);

            return providerElement != null && providerElement.ProviderTypeName != null
                       ? new ProviderServicesFactory().GetInstance(providerElement.ProviderTypeName, providerInvariantName)
                       : null;
        }

        public MigrationSqlGenerator TryGetMigrationSqlGenerator(string providerInvariantName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(providerInvariantName));

            var providerElement = TryGetProviderElement(providerInvariantName);

            if (providerElement != null
                && providerElement.SqlGeneratorElement != null
                && !string.IsNullOrWhiteSpace(providerElement.SqlGeneratorElement.SqlGeneratorTypeName))
            {
                var typeName = providerElement.SqlGeneratorElement.SqlGeneratorTypeName;
                var providerType = Type.GetType(typeName, throwOnError: false);

                if (providerType == null)
                {
                    throw new InvalidOperationException(Strings.SqlGeneratorTypeMissing(typeName, providerInvariantName));
                }
                return providerType.CreateInstance<MigrationSqlGenerator>(Strings.CreateInstance_BadSqlGeneratorType);
            }

            return null;
        }

        private ProviderElement TryGetProviderElement(string providerInvariantName)
        {
            return _entityFrameworkSettings.Providers
                .OfType<ProviderElement>()
                .FirstOrDefault(
                    e => providerInvariantName.Equals(e.InvariantName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
