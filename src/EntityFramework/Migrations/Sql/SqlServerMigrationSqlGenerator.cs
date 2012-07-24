namespace System.Data.Entity.Migrations.Sql
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Migrations.Extensions;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.Migrations.Utilities;
    using System.Data.Entity.Spatial;
    using System.Data.Entity.Utilities;
    using System.Data.SqlClient;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    ///     Provider to convert provider agnostic migration operations into SQL commands 
    ///     that can be run against a Microsoft SQL Server database.
    /// </summary>
    public class SqlServerMigrationSqlGenerator : MigrationSqlGenerator
    {
        internal const string DateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffK";
        internal const string DateTimeOffsetFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz";

        private const int DefaultMaxLength = 128;
        private const int DefaultNumericPrecision = 18;
        private const byte DefaultTimePrecision = 7;
        private const byte DefaultScale = 0;

        private DbProviderManifest _providerManifest;
        private List<MigrationStatement> _statements;
        private HashSet<string> _generatedSchemas;

        private int _variableCounter;

        /// <summary>
        ///     Converts a set of migration operations into Microsoft SQL Server specific SQL.
        /// </summary>
        /// <param name = "migrationOperations">The operations to be converted.</param>
        /// <param name = "providerManifestToken">Token representing the version of SQL Server being targeted (i.e. "2005", "2008").</param>
        /// <returns>A list of SQL statements to be executed to perform the migration operations.</returns>
        public override IEnumerable<MigrationStatement> Generate(
            IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
        {
            _statements = new List<MigrationStatement>();
            _generatedSchemas = new HashSet<string>();
            _variableCounter = 0;

            using (var connection = CreateConnection())
            {
                _providerManifest
                    = DbProviderServices.GetProviderServices(connection)
                        .GetProviderManifest(providerManifestToken);
            }

            migrationOperations.Each<dynamic>(o => Generate(o));

            return _statements;
        }

        /// <summary>
        ///     Creates an empty connection for the current provider.
        ///     Allows derived providers to use connection other than <see cref = "SqlConnection" />.
        /// </summary>
        /// <returns></returns>
        protected virtual DbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "CreateTableOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "createTableOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(CreateTableOperation createTableOperation)
        {
            Contract.Requires(createTableOperation != null);

            var parts = createTableOperation.Name.Split(new[] { '.' }, 2);

            if (parts.Length > 1)
            {
                var schema = parts[0];

                if (!schema.EqualsIgnoreCase("dbo")
                    && !_generatedSchemas.Contains(schema))
                {
                    GenerateCreateSchema(schema);

                    _generatedSchemas.Add(schema);
                }
            }

            using (var writer = Writer())
            {
                writer.WriteLine("CREATE TABLE " + Name(createTableOperation.Name) + " (");
                writer.Indent++;

                var columnCount = createTableOperation.Columns.Count();

                createTableOperation.Columns.Each(
                    (c, i) =>
                        {
                            Generate(c, writer);

                            if (i < columnCount - 1)
                            {
                                writer.WriteLine(",");
                            }
                        });

                if (createTableOperation.PrimaryKey != null)
                {
                    writer.WriteLine(",");
                    writer.Write("CONSTRAINT ");
                    writer.Write(Quote(createTableOperation.PrimaryKey.Name));
                    writer.Write(" PRIMARY KEY (");
                    writer.Write(createTableOperation.PrimaryKey.Columns.Join(Quote));
                    writer.WriteLine(")");
                }
                else
                {
                    writer.WriteLine();
                }

                writer.Indent--;
                writer.Write(")");

                Statement(writer);
            }

            GenerateMakeSystemTable(createTableOperation);
        }

        /// <summary>
        ///     Generates SQL to mark a table as a system table.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "createTableOperation">The table to mark as a system table.</param>
        protected virtual void GenerateMakeSystemTable(CreateTableOperation createTableOperation)
        {
            Contract.Requires(createTableOperation != null);

            object isMsShippedValue;
            createTableOperation.AnonymousArguments.TryGetValue("IsMSShipped", out isMsShippedValue);
            var isMsShipped = isMsShippedValue as bool?;

            if (isMsShipped == true)
            {
                using (var writer = Writer())
                {
                    writer.WriteLine("BEGIN TRY");

                    writer.Indent++;
                    writer.WriteLine("EXEC sp_MS_marksystemobject '" + createTableOperation.Name + "'");
                    writer.Indent--;

                    writer.WriteLine("END TRY");
                    writer.WriteLine("BEGIN CATCH");
                    writer.Write("END CATCH");

                    Statement(writer);
                }
            }
        }

        /// <summary>
        ///     Generates SQL to create a database schema.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "createTableOperation">The name of the schema to create.</param>
        protected virtual void GenerateCreateSchema(string schema)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(schema));

            using (var writer = Writer())
            {
                writer.Write("IF schema_id('");
                writer.Write(schema);
                writer.WriteLine("') IS NULL");
                writer.Indent++;
                writer.Write("EXECUTE('CREATE SCHEMA ");
                writer.Write(Quote(schema));
                writer.Write("')");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "AddForeignKeyOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "addForeignKeyOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(AddForeignKeyOperation addForeignKeyOperation)
        {
            Contract.Requires(addForeignKeyOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(addForeignKeyOperation.DependentTable));
                writer.Write(" ADD CONSTRAINT ");
                writer.Write(Quote(addForeignKeyOperation.Name));
                writer.Write(" FOREIGN KEY (");
                writer.Write(addForeignKeyOperation.DependentColumns.Select(Quote).Join());
                writer.Write(") REFERENCES ");
                writer.Write(Name(addForeignKeyOperation.PrincipalTable));
                writer.Write(" (");
                writer.Write(addForeignKeyOperation.PrincipalColumns.Select(Quote).Join());
                writer.Write(")");

                if (addForeignKeyOperation.CascadeDelete)
                {
                    writer.Write(" ON DELETE CASCADE");
                }

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DropForeignKeyOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "dropForeignKeyOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DropForeignKeyOperation dropForeignKeyOperation)
        {
            Contract.Requires(dropForeignKeyOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(dropForeignKeyOperation.DependentTable));
                writer.Write(" DROP CONSTRAINT ");
                writer.Write(Quote(dropForeignKeyOperation.Name));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "CreateIndexOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "createIndexOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(CreateIndexOperation createIndexOperation)
        {
            Contract.Requires(createIndexOperation != null);

            using (var writer = Writer())
            {
                writer.Write("CREATE ");

                if (createIndexOperation.IsUnique)
                {
                    writer.Write("UNIQUE ");
                }

                writer.Write("INDEX ");
                writer.Write(Quote(createIndexOperation.Name));
                writer.Write(" ON ");
                writer.Write(Name(createIndexOperation.Table));
                writer.Write("(");
                writer.Write(createIndexOperation.Columns.Join(Quote));
                writer.Write(")");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DropIndexOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "dropIndexOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DropIndexOperation dropIndexOperation)
        {
            Contract.Requires(dropIndexOperation != null);

            using (var writer = Writer())
            {
                writer.Write("DROP INDEX ");
                writer.Write(Quote(dropIndexOperation.Name));
                writer.Write(" ON ");
                writer.Write(Name(dropIndexOperation.Table));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "AddPrimaryKeyOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "addPrimaryKeyOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(AddPrimaryKeyOperation addPrimaryKeyOperation)
        {
            Contract.Requires(addPrimaryKeyOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(addPrimaryKeyOperation.Table));
                writer.Write(" ADD CONSTRAINT ");
                writer.Write(Quote(addPrimaryKeyOperation.Name));
                writer.Write(" PRIMARY KEY (");
                writer.Write(addPrimaryKeyOperation.Columns.Select(Quote).Join());
                writer.Write(")");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DropPrimaryKeyOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "dropPrimaryKeyOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DropPrimaryKeyOperation dropPrimaryKeyOperation)
        {
            Contract.Requires(dropPrimaryKeyOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(dropPrimaryKeyOperation.Table));
                writer.Write(" DROP CONSTRAINT ");
                writer.Write(Quote(dropPrimaryKeyOperation.Name));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "AddColumnOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "addColumnOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(AddColumnOperation addColumnOperation)
        {
            Contract.Requires(addColumnOperation != null);

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(addColumnOperation.Table));
                writer.Write(" ADD ");

                var column = addColumnOperation.Column;

                Generate(column, writer);

                if ((column.IsNullable != null)
                    && !column.IsNullable.Value
                    && (column.DefaultValue == null)
                    && (string.IsNullOrWhiteSpace(column.DefaultValueSql))
                    && !column.IsIdentity
                    && !column.IsTimestamp
                    && !column.StoreType.EqualsIgnoreCase("rowversion")
                    && !column.StoreType.EqualsIgnoreCase("timestamp"))
                {
                    writer.Write(" DEFAULT ");

                    if (column.Type
                        == PrimitiveTypeKind.DateTime)
                    {
                        writer.Write(Generate(DateTime.Parse("1900-01-01 00:00:00", CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        writer.Write(Generate((dynamic)column.ClrDefaultValue));
                    }
                }

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DropColumnOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "dropColumnOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DropColumnOperation dropColumnOperation)
        {
            Contract.Requires(dropColumnOperation != null);

            using (var writer = Writer())
            {
                var variable = "@var" + _variableCounter++;

                writer.Write("DECLARE ");
                writer.Write(variable);
                writer.WriteLine(" nvarchar(128)");
                writer.Write("SELECT ");
                writer.Write(variable);
                writer.WriteLine(" = name");
                writer.WriteLine("FROM sys.default_constraints");
                writer.Write("WHERE parent_object_id = object_id(N'");
                writer.Write(dropColumnOperation.Table);
                writer.WriteLine("')");
                writer.Write("AND col_name(parent_object_id, parent_column_id) = '");
                writer.Write(dropColumnOperation.Name);
                writer.WriteLine("';");
                writer.Write("IF ");
                writer.Write(variable);
                writer.WriteLine(" IS NOT NULL");
                writer.Indent++;
                writer.Write("EXECUTE('ALTER TABLE ");
                writer.Write(Name(dropColumnOperation.Table));
                writer.Write(" DROP CONSTRAINT ' + ");
                writer.Write(variable);
                writer.WriteLine(")");
                writer.Indent--;

                writer.Write("ALTER TABLE ");
                writer.Write(Name(dropColumnOperation.Table));
                writer.Write(" DROP COLUMN ");
                writer.Write(Quote(dropColumnOperation.Name));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "AlterColumnOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "alterColumnOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(AlterColumnOperation alterColumnOperation)
        {
            Contract.Requires(alterColumnOperation != null);

            var column = alterColumnOperation.Column;

            if ((column.DefaultValue != null)
                || !string.IsNullOrWhiteSpace(column.DefaultValueSql))
            {
                using (var writer = Writer())
                {
                    writer.Write("ALTER TABLE ");
                    writer.Write(Name(alterColumnOperation.Table));
                    writer.Write(" ADD CONSTRAINT DF_");
                    writer.Write(column.Name);
                    writer.Write(" DEFAULT ");
                    writer.Write(
                        (column.DefaultValue != null)
                            ? Generate((dynamic)column.DefaultValue)
                            : column.DefaultValueSql
                        );
                    writer.Write(" FOR ");
                    writer.Write(Quote(column.Name));

                    Statement(writer);
                }
            }

            using (var writer = Writer())
            {
                writer.Write("ALTER TABLE ");
                writer.Write(Name(alterColumnOperation.Table));
                writer.Write(" ALTER COLUMN ");
                writer.Write(Quote(column.Name));
                writer.Write(" ");
                writer.Write(BuildColumnType(column));

                if ((column.IsNullable != null)
                    && !column.IsNullable.Value)
                {
                    writer.Write(" NOT NULL");
                }

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DropTableOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "dropTableOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DropTableOperation dropTableOperation)
        {
            Contract.Requires(dropTableOperation != null);

            using (var writer = Writer())
            {
                writer.Write("DROP TABLE ");
                writer.Write(Name(dropTableOperation.Name));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "SqlOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "sqlOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(SqlOperation sqlOperation)
        {
            Contract.Requires(sqlOperation != null);

            Statement(sqlOperation.Sql, sqlOperation.SuppressTransaction);
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "RenameColumnOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "renameColumnOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(RenameColumnOperation renameColumnOperation)
        {
            Contract.Requires(renameColumnOperation != null);

            using (var writer = Writer())
            {
                writer.Write("EXECUTE sp_rename @objname = N'");
                writer.Write(renameColumnOperation.Table);
                writer.Write(".");
                writer.Write(renameColumnOperation.Name);
                writer.Write("', @newname = N'");
                writer.Write(renameColumnOperation.NewName);
                writer.Write("', @objtype = N'COLUMN'");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "RenameTableOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "renameTableOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(RenameTableOperation renameTableOperation)
        {
            Contract.Requires(renameTableOperation != null);

            using (var writer = Writer())
            {
                writer.Write("EXECUTE sp_rename @objname = N'");
                writer.Write(renameTableOperation.Name);
                writer.Write("', @newname = N'");
                writer.Write(renameTableOperation.NewName);
                writer.Write("', @objtype = N'OBJECT'");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "MoveTableOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "moveTableOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(MoveTableOperation moveTableOperation)
        {
            Contract.Requires(moveTableOperation != null);

            using (var writer = Writer())
            {
                var newSchema = moveTableOperation.NewSchema ?? "dbo";

                if (!newSchema.EqualsIgnoreCase("dbo")
                    && !_generatedSchemas.Contains(newSchema))
                {
                    GenerateCreateSchema(newSchema);

                    _generatedSchemas.Add(newSchema);
                }

                writer.Write("ALTER SCHEMA ");
                writer.Write(Quote(newSchema));
                writer.Write(" TRANSFER ");
                writer.Write(Name(moveTableOperation.Name));

                Statement(writer);
            }
        }

        private void Generate(ColumnModel column, IndentedTextWriter writer)
        {
            Contract.Requires(column != null);
            Contract.Requires(writer != null);

            writer.Write(Quote(column.Name));
            writer.Write(" ");
            writer.Write(BuildColumnType(column));

            if ((column.IsNullable != null)
                && !column.IsNullable.Value)
            {
                writer.Write(" NOT NULL");
            }

            if (column.DefaultValue != null)
            {
                writer.Write(" DEFAULT ");
                writer.Write(Generate((dynamic)column.DefaultValue));
            }
            else if (!string.IsNullOrWhiteSpace(column.DefaultValueSql))
            {
                writer.Write(" DEFAULT ");
                writer.Write(column.DefaultValueSql);
            }
            else if (column.IsIdentity)
            {
                if ((column.Type == PrimitiveTypeKind.Guid)
                    && (column.DefaultValue == null))
                {
                    writer.Write(" DEFAULT newid()");
                }
                else
                {
                    writer.Write(" IDENTITY");
                }
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "InsertHistoryOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "insertHistoryOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(InsertHistoryOperation insertHistoryOperation)
        {
            Contract.Requires(insertHistoryOperation != null);

            using (var writer = Writer())
            {
                writer.Write("INSERT INTO ");
                writer.Write(Name(insertHistoryOperation.Table));
                writer.Write(" ([MigrationId], [Model], [ProductVersion])");
                writer.Write(" VALUES (");
                writer.Write(Generate(insertHistoryOperation.MigrationId));
                writer.Write(", ");
                writer.Write(Generate(insertHistoryOperation.Model));
                writer.Write(", ");
                writer.Write(Generate(insertHistoryOperation.ProductVersion));
                writer.Write(")");

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL for a <see cref = "DeleteHistoryOperation" />.
        ///     Generated SQL should be added using the Statement method.
        /// </summary>
        /// <param name = "deleteHistoryOperation">The operation to produce SQL for.</param>
        protected virtual void Generate(DeleteHistoryOperation deleteHistoryOperation)
        {
            Contract.Requires(deleteHistoryOperation != null);

            using (var writer = Writer())
            {
                writer.Write("DELETE FROM ");
                writer.Write(Name(deleteHistoryOperation.Table));
                writer.Write(" WHERE [MigrationId] = ");
                writer.Write(Generate(deleteHistoryOperation.MigrationId));

                Statement(writer);
            }
        }

        /// <summary>
        ///     Generates SQL to specify a constant byte[] default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(byte[] defaultValue)
        {
            Contract.Requires(defaultValue != null);

            return "0x" + defaultValue.ToHexString();
        }

        /// <summary>
        ///     Generates SQL to specify a constant bool default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(bool defaultValue)
        {
            return defaultValue ? "1" : "0";
        }

        /// <summary>
        ///     Generates SQL to specify a constant DateTime default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(DateTime defaultValue)
        {
            return "'" + defaultValue.ToString(DateTimeFormat, CultureInfo.InvariantCulture) + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant DateTimeOffset default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(DateTimeOffset defaultValue)
        {
            return "'" + defaultValue.ToString(DateTimeOffsetFormat, CultureInfo.InvariantCulture) + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant Guid default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(Guid defaultValue)
        {
            return "'" + defaultValue + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant string default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(string defaultValue)
        {
            Contract.Assert(defaultValue != null);

            return "'" + defaultValue + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant TimeSpan default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(TimeSpan defaultValue)
        {
            return "'" + defaultValue + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant geogrpahy default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(DbGeography defaultValue)
        {
            return "'" + defaultValue + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant geometry default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(DbGeometry defaultValue)
        {
            return "'" + defaultValue + "'";
        }

        /// <summary>
        ///     Generates SQL to specify a constant default value being set on a column.
        ///     This method just generates the actual value, not the SQL to set the default value.
        /// </summary>
        /// <param name = "defaultValue">The value to be set.</param>
        /// <returns>SQL representing the default value.</returns>
        protected virtual string Generate(object defaultValue)
        {
            Contract.Assert(defaultValue != null);
            Contract.Assert(defaultValue.GetType().IsValueType);

            return defaultValue.ToString();
        }

        /// <summary>
        ///     Generates SQL to specify the data type of a column.
        ///     This method just generates the actual type, not the SQL to create the column.
        /// </summary>
        /// <param name = "defaultValue">The definition of the column.</param>
        /// <returns>SQL representing the data type.</returns>
        protected virtual string BuildColumnType(ColumnModel column)
        {
            Contract.Requires(column != null);

            if (column.IsTimestamp)
            {
                return "rowversion";
            }

            var originalStoreTypeName = column.StoreType;

            if (string.IsNullOrWhiteSpace(originalStoreTypeName))
            {
                var typeUsage = _providerManifest.GetStoreType(column.TypeUsage).EdmType;

                originalStoreTypeName = typeUsage.Name;
            }

            var storeTypeName = originalStoreTypeName;

            const string MaxSuffix = "(max)";

            if (storeTypeName.EndsWith(MaxSuffix, StringComparison.Ordinal))
            {
                storeTypeName = Quote(storeTypeName.Substring(0, storeTypeName.Length - MaxSuffix.Length)) + MaxSuffix;
            }
            else
            {
                storeTypeName = Quote(storeTypeName);
            }

            switch (originalStoreTypeName)
            {
                case "decimal":
                case "numeric":
                    storeTypeName += "(" + (column.Precision ?? DefaultNumericPrecision)
                                     + ", " + (column.Scale ?? DefaultScale) + ")";
                    break;
                case "datetime2":
                case "datetimeoffset":
                case "time":
                    storeTypeName += "(" + (column.Precision ?? DefaultTimePrecision) + ")";
                    break;
                case "binary":
                case "varbinary":
                case "nvarchar":
                case "varchar":
                case "char":
                case "nchar":
                    storeTypeName += "(" + (column.MaxLength ?? DefaultMaxLength) + ")";
                    break;
            }

            return storeTypeName;
        }

        /// <summary>
        ///     Generates a quoted name. The supplied name may or may not contain the schema.
        /// </summary>
        /// <param name = "name">The name to be quoted.</param>
        /// <returns>The quoted name.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
        protected virtual string Name(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            var parts = name.Split(new[] { '.' }, 2);

            return parts.Join(Quote, ".");
        }

        /// <summary>
        ///     Quotes an identifier for SQL Server.
        /// </summary>
        /// <param name = "identifier">The identifier to be quoted.</param>
        /// <returns>The quoted identifier.</returns>
        protected virtual string Quote(string identifier)
        {
            return "[" + identifier + "]";
        }

        /// <summary>
        ///     Adds a new Statement to be executed against the database.
        /// </summary>
        /// <param name = "sql">The statement to be executed.</param>
        /// <param name = "suppressTransaction">
        ///     Gets or sets a value indicating whether this statement should be performed outside of
        ///     the transaction scope that is used to make the migration process transactional.
        ///     If set to true, this operation will not be rolled back if the migration process fails.
        /// </param>
        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void Statement(string sql, bool suppressTransaction = false)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(sql));

            _statements.Add(
                new MigrationStatement
                    {
                        Sql = sql,
                        SuppressTransaction = suppressTransaction
                    });
        }

        /// <summary>
        ///     Gets a new <see cref = "IndentedTextWriter" /> that can be used to build SQL.
        /// 
        ///     This is just a helper method to create a writer. Writing to the writer will
        ///     not cause SQL to be registered for execution. You must pass the generated
        ///     SQL to the Statement method.
        /// </summary>
        /// <returns>An empty text writer to use for SQL generation.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        protected static IndentedTextWriter Writer()
        {
            return new IndentedTextWriter(new StringWriter(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Adds a new Statement to be executed against the database.
        /// </summary>
        /// <param name = "writer">The writer containing the SQL to be executed.</param>
        protected void Statement(IndentedTextWriter writer)
        {
            Contract.Requires(writer != null);

            Statement(writer.InnerWriter.ToString());
        }
    }
}
