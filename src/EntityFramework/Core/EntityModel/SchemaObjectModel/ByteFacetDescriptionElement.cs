namespace System.Data.Entity.Core.EntityModel.SchemaObjectModel
{
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Xml;

    internal sealed class ByteFacetDescriptionElement : FacetDescriptionElement
    {
        public ByteFacetDescriptionElement(TypeElement type, string name)
            : base(type, name)
        {
        }

        public override EdmType FacetType
        {
            get { return MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Byte); }
        }

        /////////////////////////////////////////////////////////////////////
        // Attribute Handlers

        /// <summary>
        /// Handler for the Default attribute
        /// </summary>
        /// <param name="reader">xml reader currently positioned at Default attribute</param>
        protected override void HandleDefaultAttribute(XmlReader reader)
        {
            byte value = 0;
            if (HandleByteAttribute(reader, ref value))
            {
                DefaultValue = value;
            }
        }
    }
}
