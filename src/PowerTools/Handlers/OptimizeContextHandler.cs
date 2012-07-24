﻿namespace Microsoft.DbContextPackage.Handlers
{
    using System;
    using System.Data.Entity.Design;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using EnvDTE;
    using Microsoft.DbContextPackage.Extensions;
    using Microsoft.DbContextPackage.Resources;
    using Microsoft.DbContextPackage.Utilities;
    using Task = System.Threading.Tasks.Task;

    internal class OptimizeContextHandler
    {
        private readonly DbContextPackage _package;

        public OptimizeContextHandler(DbContextPackage package)
        {
            Contract.Requires(package != null);

            _package = package;
        }

        public void OptimizeContext(dynamic context)
        {
            Type contextType = context.GetType();

            if (GetEntityFrameworkVersion(contextType) >= new Version(6, 0))
            {
                MessageBox.Show(
                    "Generating views for Entity Framework version 6 is currently not supported.",
                    "Entity Framework Power Tools",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                var selectedItem = _package.DTE2.SelectedItems.Item(1);
                var selectedItemExtension = (string)selectedItem.ProjectItem.Properties.Item("Extension").Value;
                var languageOption = selectedItemExtension == FileExtensions.CSharp
                    ? LanguageOption.GenerateCSharpCode
                    : LanguageOption.GenerateVBCode;
                var objectContext = DbContextPackage.GetObjectContext(context);
                var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);

                OptimizeContextCore(languageOption, contextType.Name, mappingCollection);
            }
            catch (Exception ex)
            {
                _package.LogError(Strings.Optimize_ContextError(contextType.Name), ex);
            }
        }

        public void OptimizeEdmx(string inputPath)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(inputPath));

            var baseFileName = Path.GetFileNameWithoutExtension(inputPath);

            try
            {
                var project = _package.DTE2.SelectedItems.Item(1).ProjectItem.ContainingProject;
                var languageOption = project.CodeModel.Language == CodeModelLanguageConstants.vsCMLanguageCSharp
                    ? LanguageOption.GenerateCSharpCode
                    : LanguageOption.GenerateVBCode;
                var mappingCollection = new EdmxUtility(inputPath).GetMappingCollection();

                OptimizeContextCore(languageOption, baseFileName, mappingCollection);
            }
            catch (Exception ex)
            {
                _package.LogError(Strings.Optimize_EdmxError(baseFileName), ex);
            }
        }

        private void OptimizeContextCore(LanguageOption languageOption, string baseFileName, StorageMappingItemCollection mappingCollection)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(baseFileName));
            Contract.Requires(mappingCollection != null);

            var progressTimer = new Timer { Interval = 1000 };

            try
            {
                var selectedItem = _package.DTE2.SelectedItems.Item(1);
                var selectedItemPath = (string)selectedItem.ProjectItem.Properties.Item("FullPath").Value;
                var viewGenerator = new EntityViewGenerator(languageOption);
                var viewsFileName = baseFileName
                        + ".Views"
                        + ((languageOption == LanguageOption.GenerateCSharpCode)
                            ? FileExtensions.CSharp
                            : FileExtensions.VisualBasic);
                var viewsPath = Path.Combine(
                    Path.GetDirectoryName(selectedItemPath),
                    viewsFileName);

                _package.DTE2.SourceControl.CheckOutItemIfNeeded(viewsPath);

                var progress = 1;
                progressTimer.Tick += (sender, e) =>
                    {
                        _package.DTE2.StatusBar.Progress(true, string.Empty, progress, 100);
                        progress = progress == 100 ? 1 : progress + 1;
                        _package.DTE2.StatusBar.Text = Strings.Optimize_Begin(baseFileName);
                    };

                progressTimer.Start();

                Task.Factory.StartNew(
                    () =>
                    {
                        var errors = viewGenerator.GenerateViews(mappingCollection, viewsPath);
                        errors.HandleErrors(Strings.Optimize_SchemaError(baseFileName));
                    })
                    .ContinueWith(
                        t =>
                        {
                            progressTimer.Stop();
                            _package.DTE2.StatusBar.Progress(false);

                            if (t.IsFaulted)
                            {
                                _package.LogError(Strings.Optimize_Error(baseFileName), t.Exception);

                                return;
                            }

                            selectedItem.ProjectItem.ContainingProject.ProjectItems.AddFromFile(viewsPath);
                            _package.DTE2.ItemOperations.OpenFile(viewsPath);

                            _package.DTE2.StatusBar.Text = Strings.Optimize_End(baseFileName, Path.GetFileName(viewsPath));
                        },
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                progressTimer.Stop();
                _package.DTE2.StatusBar.Progress(false);

                throw;
            }
        }

        private Version GetEntityFrameworkVersion(Type contextType)
        {
            Contract.Requires(contextType != null);

            while (contextType != null
                && contextType.FullName != "System.Data.Entity.DbContext"
                && contextType.Assembly.GetName().Name != "EntityFramework")
            {
                contextType = contextType.BaseType;
            }

            Contract.Assert(contextType != null);

            return contextType.Assembly.GetName().Version;
        }
    }
}