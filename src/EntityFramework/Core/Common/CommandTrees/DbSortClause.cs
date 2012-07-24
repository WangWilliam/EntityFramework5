namespace System.Data.Entity.Core.Common.CommandTrees
{
    using System.Diagnostics;

    /// <summary>
    /// Specifies a sort key that can be used as part of the sort order in a DbSortExpression.
    /// </summary>
    public sealed class DbSortClause
    {
        private readonly DbExpression _expr;
        private readonly bool _asc;
        private readonly string _coll;

        internal DbSortClause(DbExpression key, bool asc, string collation)
        {
            Debug.Assert(key != null, "DbSortClause key cannot be null");

            _expr = key;
            _asc = asc;
            _coll = collation;
        }

        /// <summary>
        /// Gets a Boolean value indicating whether or not this sort key is sorted ascending.
        /// </summary>
        public bool Ascending
        {
            get { return _asc; }
        }

        /// <summary>
        /// Gets a string value that specifies the collation for this sort key.
        /// </summary>
        public string Collation
        {
            get { return _coll; }
        }

        /// <summary>
        /// Gets the <see cref="DbExpression"/> that provides the value for this sort key.
        /// </summary>
        public DbExpression Expression
        {
            get { return _expr; }
        }
    }
}
