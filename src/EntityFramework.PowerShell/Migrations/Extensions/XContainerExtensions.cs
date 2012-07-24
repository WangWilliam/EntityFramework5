﻿namespace System.Data.Entity.Migrations.Extensions
{
    using System.Diagnostics.Contracts;
    using System.Xml.Linq;

    internal static class XContainerExtensions
    {
        public static XElement GetOrCreateElement(
            this XContainer container, string elementName, params XAttribute[] attributes)
        {
            Contract.Assert(container != null);
            Contract.Assert(!string.IsNullOrWhiteSpace(elementName));

            var element = container.Element(elementName);
            if (element == null)
            {
                element = new XElement(elementName, attributes);
                container.Add(element);
            }
            return element;
        }
    }
}
