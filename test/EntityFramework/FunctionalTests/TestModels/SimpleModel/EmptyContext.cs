﻿namespace SimpleModel
{
    using System.Data.Entity.Core.Common;
    using System.Data.Common;
    using System.Data.Entity;

    public class EmptyContext : DbContext
    {
        public EmptyContext()
        {
        }

        public EmptyContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public EmptyContext(DbConnection connection, bool contextOwnsConnection = false)
            : base(connection, contextOwnsConnection)
        {
        }
    }
}