namespace System.Data.Entity.Core.Mapping
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Describes modification function mappings for an association set.
    /// </summary>
    internal sealed class StorageAssociationSetModificationFunctionMapping
    {
        internal StorageAssociationSetModificationFunctionMapping(
            AssociationSet associationSet,
            StorageModificationFunctionMapping deleteFunctionMapping,
            StorageModificationFunctionMapping insertFunctionMapping)
        {
            Contract.Requires(associationSet != null);

            AssociationSet = associationSet;
            DeleteFunctionMapping = deleteFunctionMapping;
            InsertFunctionMapping = insertFunctionMapping;
        }

        /// <summary>
        /// Association set these functions handles.
        /// </summary>
        internal readonly AssociationSet AssociationSet;

        /// <summary>
        /// Delete function for this association set.
        /// </summary>
        internal readonly StorageModificationFunctionMapping DeleteFunctionMapping;

        /// <summary>
        /// Insert function for this association set.
        /// </summary>
        internal readonly StorageModificationFunctionMapping InsertFunctionMapping;

        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "AS{{{0}}}:{3}DFunc={{{1}}},{3}IFunc={{{2}}}", AssociationSet, DeleteFunctionMapping,
                InsertFunctionMapping, Environment.NewLine + "  ");
        }

#if DEBUG
        internal void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            var sb = new StringBuilder();
            sb.Append("Association Set Function Mapping");
            sb.Append("   ");
            sb.Append(ToString());
            Console.WriteLine(sb.ToString());
        }
#endif
    }
}
