namespace System.Data.Entity.ModelConfiguration.Configuration
{
    using System.Data.Entity.Edm.Db;
    using System.Data.Entity.Edm.Db.Mapping;
    using System.Diagnostics.Contracts;

    /// <summary>
    ///     Base class for performing configuration of a relationship.
    ///     This configuration functionality is available via the Code First Fluent API, see <see cref = "DbModelBuilder" />.
    /// </summary>
    [ContractClass(typeof(AssociationMappingConfigurationContracts))]
    public abstract class AssociationMappingConfiguration
    {
        internal abstract void Configure(DbAssociationSetMapping associationSetMapping, DbDatabaseMetadata database);

        internal abstract AssociationMappingConfiguration Clone();

        #region Base Member Contracts

        [ContractClassFor(typeof(AssociationMappingConfiguration))]
        private abstract class AssociationMappingConfigurationContracts : AssociationMappingConfiguration
        {
            internal override void Configure(DbAssociationSetMapping associationSetMapping, DbDatabaseMetadata database)
            {
                Contract.Requires(associationSetMapping != null);
                Contract.Requires(database != null);
            }
        }

        #endregion
    }
}
