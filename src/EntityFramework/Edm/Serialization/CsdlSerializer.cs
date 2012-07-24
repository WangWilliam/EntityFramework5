namespace System.Data.Entity.Edm.Serialization
{
    using System.Data.Entity.Edm.Common;
    using System.Data.Entity.Edm.Parsing.Xml.Internal;
    using System.Data.Entity.Edm.Serialization.Xml.Internal.Csdl;
    using System.Data.Entity.Edm.Validation.Internal;
    using System.Data.Entity.Resources;
    using System.Diagnostics.Contracts;
    using System.Xml;

    /// <summary>
    ///     Serializes an <see cref = "EdmModel" /> that conforms to the restrictions of a single CSDL schema file to an XML writer. The model to be serialized must contain a single <see cref = "EdmNamespace" /> and a single <see cref = "EdmEntityContainer" /> .
    /// </summary>
    internal class CsdlSerializer
    {
        private bool _isModelValid = true;

        public event EventHandler<DataModelErrorEventArgs> OnError;

        /// <summary>
        ///     Serialize the <see cref = "EdmModel" /> to the XmlWriter.
        /// </summary>
        /// <param name = "model"> The EdmModel to serialize, mut have only one <see cref = "EdmNamespace" /> and one <see cref = "EdmEntityContainer" /> </param>
        /// <param name = "xmlWriter"> The XmlWriter to serialize to </param>
        public bool Serialize(EdmModel model, XmlWriter xmlWriter)
        {
            Contract.Requires(model != null);
            Contract.Requires(xmlWriter != null);

            if (model.Namespaces.Count != 1
                || model.Containers.Count != 1)
            {
                Validator_OnError(
                    this,
                    new DataModelErrorEventArgs
                        {
                            ErrorMessage = Strings.Serializer_OneNamespaceAndOneContainer,
                            ErrorCode = XmlErrorCode.Serializer_OneNamespaceAndOneContainer
                        });
            }

            // validate the model first
            var validator = new DataModelValidator();
            validator.OnError += Validator_OnError;
            validator.Validate(model, true);

            if (_isModelValid)
            {
                var visitor = new EdmModelCsdlSerializationVisitor(xmlWriter, model.Version);
                visitor.Visit(model);
            }
            return _isModelValid;
        }

        private void Validator_OnError(object sender, DataModelErrorEventArgs e)
        {
            _isModelValid = false;
            if (OnError != null)
            {
                OnError(sender, e);
            }
        }
    }
}
