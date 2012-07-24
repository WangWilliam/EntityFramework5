namespace System.Data.Entity.Edm.Serialization
{
    using System.Data.Entity.Edm.Db;
    using System.Data.Entity.Edm.Serialization.Xml.Internal.Ssdl;
    using System.Diagnostics.Contracts;
    using System.Xml;

    internal class SsdlSerializer
    {
        /// <summary>
        ///     Serialize the <see cref = "DbDatabaseMetadata" /> to the <see cref = "XmlWriter" />
        /// </summary>
        /// <param name = "dbDatabase"> The DbDatabaseMetadata to serialize </param>
        /// <param name = "provider"> Provider information on the Schema element </param>
        /// <param name = "providerManifestToken"> ProviderManifestToken information on the Schema element </param>
        /// <param name = "xmlWriter"> The XmlWriter to serialize to </param>
        /// <returns> </returns>
        public virtual bool Serialize(
            DbDatabaseMetadata dbDatabase, string provider, string providerManifestToken, XmlWriter xmlWriter)
        {
            Contract.Requires(dbDatabase != null);
            Contract.Requires(xmlWriter != null);

            // validate the model first
            // TODO: Add the validation code once the DbSchema Validator is done.

            var visitor = new DbModelSsdlSerializationVisitor(xmlWriter, dbDatabase.Version);

            visitor.Visit(dbDatabase, provider, providerManifestToken);

            return true;
        }
    }
}
