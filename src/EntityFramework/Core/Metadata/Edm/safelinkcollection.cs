﻿namespace System.Data.Entity.Core.Metadata.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// This class attempts to make a double linked connection between a parent and child without
    /// exposing the properties publicly that would allow them to be mutable and possibly dangerous
    /// in a multithreading environment
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    /// <typeparam name="TChild"></typeparam>
    internal class SafeLinkCollection<TParent, TChild> : ReadOnlyMetadataCollection<TChild>
        where TChild : MetadataItem
        where TParent : class
    {
        public SafeLinkCollection(TParent parent, Func<TChild, SafeLink<TParent>> getLink, MetadataCollection<TChild> children)
            : base((IList<TChild>)SafeLink<TParent>.BindChildren(parent, getLink, children))
        {
        }
    }
}
