namespace System.Data.Entity.Core.Common.CommandTrees
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Diagnostics;

    /// <summary>
    /// Represents the projection of a given set of values over the specified input set.
    /// </summary>
    public sealed class DbProjectExpression : DbExpression
    {
        private readonly DbExpressionBinding _input;
        private readonly DbExpression _projection;

        internal DbProjectExpression(TypeUsage resultType, DbExpressionBinding input, DbExpression projection)
            : base(DbExpressionKind.Project, resultType)
        {
            Debug.Assert(input != null, "DbProjectExpression input cannot be null");
            Debug.Assert(projection != null, "DbProjectExpression projection cannot be null");

            _input = input;
            _projection = projection;
        }

        /// <summary>
        /// Gets the <see cref="DbExpressionBinding"/> that specifies the input set.
        /// </summary>
        public DbExpressionBinding Input
        {
            get { return _input; }
        }

        /// <summary>
        /// Gets the <see cref="DbExpression"/> that defines the projection.
        /// </summary>
        public DbExpression Projection
        {
            get { return _projection; }
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
