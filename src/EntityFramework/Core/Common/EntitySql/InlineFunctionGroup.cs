namespace System.Data.Entity.Core.Common.EntitySql
{
    using System.Collections.Generic;
    using System.Data.Entity.Resources;
    using System.Diagnostics;

    /// <summary>
    /// Represents an eSQL metadata member expression classified as <see cref="MetadataMemberClass.InlineFunctionGroup"/>.
    /// </summary>
    internal sealed class InlineFunctionGroup : MetadataMember
    {
        internal InlineFunctionGroup(string name, IList<InlineFunctionInfo> functionMetadata)
            : base(MetadataMemberClass.InlineFunctionGroup, name)
        {
            Debug.Assert(functionMetadata != null && functionMetadata.Count > 0, "FunctionMetadata must not be null or empty");
            FunctionMetadata = functionMetadata;
        }

        internal override string MetadataMemberClassName
        {
            get { return InlineFunctionGroupClassName; }
        }

        internal static string InlineFunctionGroupClassName
        {
            get { return Strings.LocalizedInlineFunction; }
        }

        internal readonly IList<InlineFunctionInfo> FunctionMetadata;
    }
}
