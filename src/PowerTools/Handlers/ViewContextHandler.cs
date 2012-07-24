﻿namespace Microsoft.DbContextPackage.Handlers
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Microsoft.DbContextPackage.Resources;
    using Microsoft.DbContextPackage.Utilities;

    internal class ViewContextHandler
    {
        private readonly DbContextPackage _package;

        public ViewContextHandler(DbContextPackage package)
        {
            Contract.Requires(package != null);

            _package = package;
        }

        public void ViewContext(MenuCommand menuCommand, dynamic context, Type systemContextType)
        {
            Contract.Requires(menuCommand != null);
            Contract.Requires(systemContextType != null);

            Type contextType = context.GetType();

            try
            {
                var filePath = Path.Combine(
                    Path.GetTempPath(),
                    contextType.Name
                        + (menuCommand.CommandID.ID == PkgCmdIDList.cmdidViewEntityDataModel
                            ? FileExtensions.EntityDataModel
                            : FileExtensions.Xml));

                if (File.Exists(filePath))
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                }

                using (var fileStream = File.Create(filePath))
                {
                    using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
                    {
                        var edmxWriterType = systemContextType.Assembly.GetType("System.Data.Entity.Infrastructure.EdmxWriter");

                        if (edmxWriterType != null)
                        {
                            edmxWriterType.InvokeMember(
                                "WriteEdmx",
                                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                                null,
                                null,
                                new object[] { context, xmlWriter });
                        }
                    }
                }

                _package.DTE2.ItemOperations.OpenFile(filePath);

                File.SetAttributes(filePath, FileAttributes.ReadOnly);
            }
            catch (Exception exception)
            {
                _package.LogError(Strings.ViewContextError(contextType.Name), exception);
            }
        }
    }
}