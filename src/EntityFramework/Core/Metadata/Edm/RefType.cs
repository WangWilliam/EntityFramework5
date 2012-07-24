namespace System.Data.Entity.Core.Metadata.Edm
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    /// <summary>
    /// Class representing a ref type
    /// </summary>
    public class RefType : EdmType
    {
        #region Constructors

        internal RefType()
        {
        }

        /// <summary>
        /// The constructor for constructing a RefType object with the entity type it references
        /// </summary>
        /// <param name="entityType">The entity type that this ref type references</param>
        /// <exception cref="System.ArgumentNullException">Thrown if entityType argument is null</exception>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal RefType(EntityType entityType)
            : base(GetIdentity(EntityUtil.GenericCheckArgumentNull(entityType, "entityType")),
                EdmConstants.TransientNamespace, entityType.DataSpace)
        {
            _elementType = entityType;
            SetReadOnly();
        }

        #endregion

        #region Fields

        private readonly EntityTypeBase _elementType;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the kind of the type
        /// </summary>
        public override BuiltInTypeKind BuiltInTypeKind
        {
            get { return BuiltInTypeKind.RefType; }
        }

        /// <summary>
        /// The entity type that this ref type references
        /// </summary>
        [MetadataProperty(BuiltInTypeKind.EntityTypeBase, false)]
        public virtual EntityTypeBase ElementType
        {
            get { return _elementType; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Constructs the name of the collection type
        /// </summary>
        /// <param name="entityTypeBase">The entity type base that this ref type refers to</param>
        /// <returns>The identity of the resulting ref type</returns>
        private static string GetIdentity(EntityTypeBase entityTypeBase)
        {
            var builder = new StringBuilder(50);
            builder.Append("reference[");
            entityTypeBase.BuildIdentity(builder);
            builder.Append("]");
            return builder.ToString();
        }

        #endregion
    }
}
