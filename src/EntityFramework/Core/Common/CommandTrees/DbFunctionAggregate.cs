namespace System.Data.Entity.Core.Common.CommandTrees
{
    using System.Data.Entity.Core.Common.CommandTrees.Internal;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// The aggregate type that corresponds to the invocation of an aggregate function.
    /// </summary>
    public sealed class DbFunctionAggregate : DbAggregate
    {
        private readonly bool _distinct;
        private readonly EdmFunction _aggregateFunction;

        internal DbFunctionAggregate(TypeUsage resultType, DbExpressionList arguments, EdmFunction function, bool isDistinct)
            : base(resultType, arguments)
        {
            Debug.Assert(function != null, "DbFunctionAggregate.Function cannot be null");

            _aggregateFunction = function;
            _distinct = isDistinct;
        }

        /// <summary>
        /// Gets a value indicating whether the aggregate function is applied in a distinct fashion
        /// </summary>
        public bool Distinct
        {
            get { return _distinct; }
        }

        /// <summary>
        /// Gets the method metadata that specifies the aggregate function to invoke.
        /// </summary>
        public EdmFunction Function
        {
            get { return _aggregateFunction; }
        }
    }
}
