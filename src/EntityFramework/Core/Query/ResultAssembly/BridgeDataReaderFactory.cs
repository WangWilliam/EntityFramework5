﻿namespace System.Data.Entity.Core.Query.ResultAssembly
{
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity.Core.Common.Internal.Materialization;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.Query.InternalTrees;
    using System.Diagnostics.Contracts;

    internal class BridgeDataReaderFactory
    {
        private readonly Translator _translator;

        public BridgeDataReaderFactory(Translator translator = null)
        {
            _translator = translator ?? new Translator();
        }

        /// <summary>
        /// The primary factory method to produce the BridgeDataReader; given a store data 
        /// reader and a column map, create the BridgeDataReader, hooking up the IteratorSources  
        /// and ResultColumn Hierarchy.  All construction of top level data readers go through
        /// this method.
        /// </summary>
        /// <param name="storeDataReader"></param>
        /// <param name="columnMap">column map of the first result set</param>
        /// <param name="nextResultColumnMaps">enumerable of the column maps for NextResult() calls.</param>
        /// <returns></returns>
        public virtual DbDataReader Create(
            DbDataReader storeDataReader, ColumnMap columnMap, MetadataWorkspace workspace, IEnumerable<ColumnMap> nextResultColumnMaps)
        {
            Contract.Requires(storeDataReader != null);
            Contract.Requires(columnMap != null);
            Contract.Requires(workspace != null);
            Contract.Requires(nextResultColumnMaps != null);

            var shaperInfo = CreateShaperInfo(storeDataReader, columnMap, workspace);
            DbDataReader result = new BridgeDataReader(
                shaperInfo.Key, shaperInfo.Value, /*depth:*/ 0,
                GetNextResultShaperInfo(storeDataReader, workspace, nextResultColumnMaps).GetEnumerator());
            return result;
        }

        private KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>> CreateShaperInfo(
            DbDataReader storeDataReader, ColumnMap columnMap, MetadataWorkspace workspace)
        {
            Contract.Requires(storeDataReader != null);
            Contract.Requires(columnMap != null);
            Contract.Requires(workspace != null);

            var cacheManager = workspace.GetQueryCacheManager();
            const MergeOption NoTracking = MergeOption.NoTracking;

            var shaperFactory = _translator.TranslateColumnMap<RecordState>(cacheManager, columnMap, workspace, null, NoTracking, true);
            var recordShaper = shaperFactory.Create(storeDataReader, null, workspace, MergeOption.NoTracking, true);

            return new KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>(
                recordShaper, recordShaper.RootCoordinator.TypedCoordinatorFactory);
        }

        private IEnumerable<KeyValuePair<Shaper<RecordState>, CoordinatorFactory<RecordState>>> GetNextResultShaperInfo(
            DbDataReader storeDataReader, MetadataWorkspace workspace, IEnumerable<ColumnMap> nextResultColumnMaps)
        {
            foreach (var nextResultColumnMap in nextResultColumnMaps)
            {
                yield return CreateShaperInfo(storeDataReader, nextResultColumnMap, workspace);
            }
        }
    }
}
