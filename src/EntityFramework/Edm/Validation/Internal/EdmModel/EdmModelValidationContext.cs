namespace System.Data.Entity.Edm.Validation.Internal.EdmModel
{
    using System.Data.Entity.Edm.Common;
    using System.Data.Entity.Edm.Internal;
    using System.Diagnostics.Contracts;
    using EdmModel = System.Data.Entity.Edm.EdmModel;

    /// <summary>
    ///     The context for EdmModel Validation
    /// </summary>
    internal sealed class EdmModelValidationContext : DataModelValidationContext
    {
        internal event EventHandler<DataModelErrorEventArgs> OnError;

        internal EdmModelValidationContext(bool validateSyntax)
        {
            ValidateSyntax = validateSyntax;
        }

        internal EdmModelParentMap ModelParentMap { get; private set; }

        internal string GetQualifiedPrefix(EdmNamespaceItem item)
        {
            Contract.Assert(
                ModelParentMap != null, "Model parent map was not initialized before calling GetQualifiedPrefix?");

            string qualifiedPrefix = null;
            EdmNamespace parentNamespace;
            if (ModelParentMap.TryGetNamespace(item, out parentNamespace))
            {
                qualifiedPrefix = parentNamespace.Name;
            }

            return qualifiedPrefix;
        }

        internal string GetQualifiedPrefix(EdmEntityContainerItem item)
        {
            Contract.Assert(
                ModelParentMap != null, "Model parent map was not initialized before calling GetQualifiedPrefix?");

            string qualifiedPrefix = null;
            EdmEntityContainer parentContainer = null;
            if (ModelParentMap.TryGetEntityContainer(item, out parentContainer))
            {
                qualifiedPrefix = parentContainer.Name;
            }

            return qualifiedPrefix;
        }

        internal void RaiseDataModelValidationEvent(DataModelErrorEventArgs error)
        {
            if (OnError != null)
            {
                OnError(this, error);
            }
        }

        internal void Validate(EdmModel root)
        {
            Contract.Assert(root != null, "root cannot be null");

            ModelParentMap = new EdmModelParentMap(root);
            ModelParentMap.Compute();

            ValidationContextVersion = root.Version;

            EdmModelValidator.Validate(root, this);
        }

        internal override void AddError(DataModelItem item, string propertyName, string errorMessage, int errorCode)
        {
            RaiseDataModelValidationEvent(
                new DataModelErrorEventArgs
                    {
                        ErrorCode = errorCode,
                        ErrorMessage = errorMessage,
                        Item = item,
                        PropertyName = propertyName,
                    }
                );
        }

        internal override void AddWarning(DataModelItem item, string propertyName, string errorMessage, int errorCode)
        {
            throw new NotImplementedException();
        }
    }
}
