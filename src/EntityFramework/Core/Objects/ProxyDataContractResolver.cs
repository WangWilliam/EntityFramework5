﻿namespace System.Data.Entity.Core.Objects
{
    using System.Data.Entity.Internal;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// A DataContractResolver that knows how to resolve proxy types created for persistent
    /// ignorant classes to their base types. This is used with the DataContractSerializer.
    /// </summary>
    public class ProxyDataContractResolver : DataContractResolver
    {
        private readonly XsdDataContractExporter _exporter = new XsdDataContractExporter();

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            DbHelpers.ThrowIfNullOrWhitespace(typeName, "typeName");
            DbHelpers.ThrowIfNullOrWhitespace(typeNamespace, "typeNamespace");
            DbHelpers.ThrowIfNull(declaredType, "declaredType");
            DbHelpers.ThrowIfNull(knownTypeResolver, "knownTypeResolver");

            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
        }

        public override bool TryResolveType(
            Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName,
            out XmlDictionaryString typeNamespace)
        {
            DbHelpers.ThrowIfNull(type, "type");
            DbHelpers.ThrowIfNull(declaredType, "declaredType");
            DbHelpers.ThrowIfNull(knownTypeResolver, "knownTypeResolver");

            var nonProxyType = ObjectContext.GetObjectType(type);
            if (nonProxyType != type)
            {
                // Type was a proxy type, so map the name to the non-proxy name
                var qualifiedName = _exporter.GetSchemaTypeName(nonProxyType);
                var dictionary = new XmlDictionary(2);
                typeName = new XmlDictionaryString(dictionary, qualifiedName.Name, 0);
                typeNamespace = new XmlDictionaryString(dictionary, qualifiedName.Namespace, 1);
                return true;
            }
            else
            {
                // Type was not a proxy type, so do the default
                return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
            }
        }
    }
}
