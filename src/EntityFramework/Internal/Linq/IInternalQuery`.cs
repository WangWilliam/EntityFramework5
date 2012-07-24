﻿namespace System.Data.Entity.Internal.Linq
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Objects.ELinq;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    /// <summary>
    ///     An interface implemented by <see cref = "InternalQuery{TElement}" />.
    /// </summary>
    /// <typeparam name = "TElement">The type of the element.</typeparam>
    [ContractClass(typeof(IInternalQueryContracts<>))]
    internal interface IInternalQuery<TElement> : IInternalQuery
    {
        IInternalQuery<TElement> Include(string path);
        IInternalQuery<TElement> AsNoTracking();
        new IDbAsyncEnumerator<TElement> GetAsyncEnumerator();
        new IEnumerator<TElement> GetEnumerator();
    }

    [ContractClassFor(typeof(IInternalQuery<>))]
    internal abstract class IInternalQueryContracts<TElement> : IInternalQuery<TElement>
    {
        void IInternalQuery.ResetQuery()
        {
            throw new NotImplementedException();
        }

        InternalContext IInternalQuery.InternalContext
        {
            get { throw new NotImplementedException(); }
        }

        ObjectQuery IInternalQuery.ObjectQuery
        {
            get { throw new NotImplementedException(); }
        }

        Type IInternalQuery.ElementType
        {
            get { throw new NotImplementedException(); }
        }

        Expression IInternalQuery.Expression
        {
            get { throw new NotImplementedException(); }
        }

        ObjectQueryProvider IInternalQuery.ObjectQueryProvider
        {
            get { throw new NotImplementedException(); }
        }

        IInternalQuery<TElement> IInternalQuery<TElement>.Include(string path)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(path));

            throw new NotImplementedException();
        }

        IInternalQuery<TElement> IInternalQuery<TElement>.AsNoTracking()
        {
            throw new NotImplementedException();
        }

        IDbAsyncEnumerator<TElement> IInternalQuery<TElement>.GetAsyncEnumerator()
        {
            throw new NotImplementedException();
        }

        IDbAsyncEnumerator IInternalQuery.GetAsyncEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<TElement> IInternalQuery<TElement>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IInternalQuery.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
