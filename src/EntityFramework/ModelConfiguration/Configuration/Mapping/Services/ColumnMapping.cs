﻿namespace System.Data.Entity.ModelConfiguration.Configuration.Mapping
{
    using System.Collections.Generic;
    using System.Data.Entity.Edm;
    using System.Data.Entity.Edm.Db;
    using System.Data.Entity.Edm.Db.Mapping;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Linq;

    [DebuggerDisplay("{Column.Name}")]
    internal class ColumnMapping
    {
        private readonly DbTableColumnMetadata _column;
        private readonly List<PropertyMappingSpecification> _propertyMappings;

        public ColumnMapping(DbTableColumnMetadata column)
        {
            Contract.Requires(column != null);
            _column = column;
            _propertyMappings = new List<PropertyMappingSpecification>();
        }

        public DbTableColumnMetadata Column
        {
            get { return _column; }
        }

        public IList<PropertyMappingSpecification> PropertyMappings
        {
            get { return _propertyMappings; }
        }

        public void AddMapping(
            EdmEntityType entityType,
            IList<EdmProperty> propertyPath,
            IEnumerable<DbColumnCondition> conditions,
            bool isDefaultDiscriminatorCondition)
        {
            _propertyMappings.Add(
                new PropertyMappingSpecification(
                    entityType, propertyPath, conditions.ToList(), isDefaultDiscriminatorCondition));
        }
    }
}
