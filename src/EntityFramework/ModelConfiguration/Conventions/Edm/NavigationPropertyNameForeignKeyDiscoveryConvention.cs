namespace System.Data.Entity.ModelConfiguration.Conventions
{
    using System.Data.Entity.Edm;
    using System.Data.Entity.ModelConfiguration.Edm;
    using System.Linq;

    /// <summary>
    ///     Convention to discover foreign key properties whose names are a combination
    ///     of the dependent navigation property name and the principal type primary key property name(s).
    /// </summary>
    public sealed class NavigationPropertyNameForeignKeyDiscoveryConvention : IEdmConvention<EdmAssociationType>
    {
        private readonly IEdmConvention<EdmAssociationType> _impl =
            new NavigationPropertyNameForeignKeyDiscoveryConventionImpl();

        internal NavigationPropertyNameForeignKeyDiscoveryConvention()
        {
        }

        void IEdmConvention<EdmAssociationType>.Apply(EdmAssociationType associationType, EdmModel model)
        {
            _impl.Apply(associationType, model);
        }

        // Nested impl. because ForeignKeyDiscoveryConvention needs to be internal for now
        private sealed class NavigationPropertyNameForeignKeyDiscoveryConventionImpl : ForeignKeyDiscoveryConvention
        {
            protected override bool MatchDependentKeyProperty(
                EdmAssociationType associationType,
                EdmAssociationEnd dependentAssociationEnd,
                EdmProperty dependentProperty,
                EdmEntityType principalEntityType,
                EdmProperty principalKeyProperty)
            {
                var otherEnd = associationType.GetOtherEnd(dependentAssociationEnd);

                var navigationProperty
                    = dependentAssociationEnd.EntityType.NavigationProperties
                        .SingleOrDefault(n => n.ResultEnd == otherEnd);

                if (navigationProperty == null)
                {
                    return false;
                }

                return string.Equals(
                    dependentProperty.Name, navigationProperty.Name + principalKeyProperty.Name,
                    StringComparison.OrdinalIgnoreCase);
            }

            protected override bool SupportsMultipleAssociations
            {
                get { return true; }
            }
        }
    }
}
