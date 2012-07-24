﻿namespace System.Data.Entity
{
    using System.Data.Common;
    using System.Reflection;

    public class GenericProviderFactory<T> : DbProviderFactory
        where T : DbProviderFactory
    {
        public static GenericProviderFactory<T> Instance = new GenericProviderFactory<T>();

        private GenericProviderFactory()
        {
            var providerTable = (DataTable)typeof(DbProviderFactories).GetMethod("GetProviderTable", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            var row = providerTable.NewRow();
            row["Name"] = "GenericProviderFactory";
            row["InvariantName"] = "My.Generic.Provider." + typeof(T).Name;
            row["Description"] = "Fake GenericProviderFactory";
            row["AssemblyQualifiedName"] = GetType().AssemblyQualifiedName;
            providerTable.Rows.Add(row);
        }

        public override DbConnection CreateConnection()
        {
            return new GenericConnection<T>();
        }
    }
}