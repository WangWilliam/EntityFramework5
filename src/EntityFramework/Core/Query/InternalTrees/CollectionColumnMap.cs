namespace System.Data.Entity.Core.Query.InternalTrees
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// Represents a column map for a collection column.
    /// The "element" represents the element of the collection - usually a Structured
    /// type, but occasionally a collection/simple type as well.
    /// The "ForeignKeys" property is optional (but usually necessary) to determine the 
    /// elements of the collection.
    /// </summary>
    internal abstract class CollectionColumnMap : ColumnMap
    {
        private readonly ColumnMap m_element;
        private readonly SimpleColumnMap[] m_foreignKeys;
        private readonly SimpleColumnMap[] m_keys;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">datatype of column</param>
        /// <param name="name">column name</param>
        /// <param name="elementMap">column map for collection element</param>
        /// <param name="keys">List of keys</param>
        /// <param name="foreignKeys">List of foreign keys</param>
        internal CollectionColumnMap(
            TypeUsage type, string name, ColumnMap elementMap, SimpleColumnMap[] keys, SimpleColumnMap[] foreignKeys)
            : base(type, name)
        {
            Debug.Assert(elementMap != null, "Must specify column map for element");

            m_element = elementMap;
            m_keys = keys ?? new SimpleColumnMap[0];
            m_foreignKeys = foreignKeys ?? new SimpleColumnMap[0];
        }

        /// <summary>
        /// Get the list of columns that may comprise the foreign key
        /// </summary>
        internal SimpleColumnMap[] ForeignKeys
        {
            get { return m_foreignKeys; }
        }

        /// <summary>
        /// Get the list of columns that may comprise the key
        /// </summary>
        internal SimpleColumnMap[] Keys
        {
            get { return m_keys; }
        }

        /// <summary>
        /// Get the column map describing the collection element
        /// </summary>
        internal ColumnMap Element
        {
            get { return m_element; }
        }
    }
}
