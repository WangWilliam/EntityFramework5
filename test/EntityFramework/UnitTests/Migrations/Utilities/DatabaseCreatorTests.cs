﻿namespace System.Data.Entity.Migrations
{
    using System.Data.Entity.Migrations.Utilities;
    using Xunit;

    [Variant(DatabaseProvider.SqlClient, ProgrammingLanguage.CSharp)]
    [Variant(DatabaseProvider.SqlServerCe, ProgrammingLanguage.CSharp)]
    public class DatabaseCreatorTests : DbTestCase
    {
        [MigrationsTheory]
        public void Create_can_create_database()
        {
            DropDatabase();

            Assert.False(DatabaseExists());

            using (var connection = ProviderFactory.CreateConnection())
            {
                connection.ConnectionString = ConnectionString;

                new DatabaseCreator().Create(connection);
            }

            Assert.True(DatabaseExists());
        }
    }
}