namespace System.Data.Entity.Core.Common.CommandTrees
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// Represents an apply operation, which is the invocation of the specified functor for each element in the specified input set.
    /// </summary>
    public sealed class DbApplyExpression : DbExpression
    {
        private readonly DbExpressionBinding _input;
        private readonly DbExpressionBinding _apply;

        internal DbApplyExpression(
            DbExpressionKind applyKind, TypeUsage resultRowCollectionTypeUsage, DbExpressionBinding input, DbExpressionBinding apply)
            : base(applyKind, resultRowCollectionTypeUsage)
        {
            Debug.Assert(input != null, "DbApplyExpression input cannot be null");
            Debug.Assert(input != null, "DbApplyExpression apply cannot be null");
            Debug.Assert(
                DbExpressionKind.CrossApply == applyKind || DbExpressionKind.OuterApply == applyKind,
                "Invalid DbExpressionKind for DbApplyExpression");

            _input = input;
            _apply = apply;
        }

        /// <summary>
        /// Gets the <see cref="DbExpressionBinding"/> that specifies the functor that is invoked for each element in the input set.
        /// </summary>
        public DbExpressionBinding Apply
        {
            get { return _apply; }
        }

        /// <summary>
        /// Gets the <see cref="DbExpressionBinding"/> that specifies the input set.
        /// </summary>
        public DbExpressionBinding Input
        {
            get { return _input; }
        }

        /// <summary>
        /// The visitor pattern method for expression visitors that do not produce a result value.
        /// </summary>
        /// <param name="visitor">An instance of DbExpressionVisitor.</param>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is null</exception>
        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor != null)
            {
                visitor.Visit(this);
            }
            else
            {
                throw new ArgumentNullException("visitor");
            }
        }

        /// <summary>
        /// The visitor pattern method for expression visitors that produce a result value of a specific type.
        /// </summary>
        /// <param name="visitor">An instance of a typed DbExpressionVisitor that produces a result value of type TResultType.</param>
        /// <typeparam name="TResultType">The type of the result produced by <paramref name="visitor"/></typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is null</exception>
        /// <returns>An instance of <typeparamref name="TResultType"/>.</returns>
        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor)
        {
            if (visitor != null)
            {
                return visitor.Visit(this);
            }
            else
            {
                throw new ArgumentNullException("visitor");
            }
        }
    }
}
