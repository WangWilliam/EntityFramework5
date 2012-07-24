namespace System.Data.Entity.Edm.Common
{
    /// <summary>
    ///     Allows the construction and modification of a user-specified annotation (name-value pair) on a <see cref = "DataModelItem" /> instance.
    /// </summary>
    internal class DataModelAnnotation
        : INamedDataModelItem
    {
        /// <summary>
        ///     Gets or sets an optional namespace that can be used to distinguish the annotation from others with the same <see cref = "Name" /> value.
        /// </summary>
        public virtual string Namespace { get; set; }

        /// <summary>
        ///     Gets or sets the name of the annotation.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value of the annotation.
        /// </summary>
        public virtual object Value { get; set; }
    }
}
