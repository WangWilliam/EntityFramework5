namespace System.Data.Entity.Edm
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The base for all all Entity Data Model (EDM) types that represent a type from the EDM type system.
    /// </summary>
    [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
    internal abstract class EdmDataModelType
        : EdmNamespaceItem
    {
        /// <summary>
        ///     Gets a value indicating whether this type is abstract.
        /// </summary>
        public bool IsAbstract { get; internal set; }

        /// <summary>
        ///     Gets the optional base type of this type.
        /// </summary>
        public EdmDataModelType BaseType { get; internal set; }
    }
}
