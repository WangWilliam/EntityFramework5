namespace System.Data.Entity.Core.Query.ResultAssembly
{
    using System.Data.Common;
    using System.Data.Entity.Core.Common.QueryCache;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Query.InternalTrees;
    using Moq;
    using Xunit;

    public class BridgeDataReaderFactoryTests
    {
        [Fact]
        public void Create_returns_a_new_instance_of_BridgeDataReader_with_two_resultsets()
        {
            var bridgeDataReaderFactory = new BridgeDataReaderFactory(Common.Internal.Materialization.MockHelper.CreateRecordStateTranslator());

            var dbDataReaderMock = new Mock<DbDataReader>();
            bool hasResult = true;
            dbDataReaderMock.Setup(m => m.NextResult()).Returns(() =>
            {
                var result = hasResult;
                hasResult = false;
                return result;
            });

            var columnMapMock = new Mock<ColumnMap>(new Mock<TypeUsage>().Object, string.Empty);

            var metadataWorkspaceMock = new Mock<MetadataWorkspace>();
            metadataWorkspaceMock.Setup(m => m.GetQueryCacheManager()).Returns(QueryCacheManager.Create());

            var bridgeDataReader = bridgeDataReaderFactory.Create(dbDataReaderMock.Object, columnMapMock.Object,
                metadataWorkspaceMock.Object, new[] { columnMapMock.Object });

            Assert.NotNull(bridgeDataReader);
            Assert.True(bridgeDataReader.NextResult());
            Assert.False(bridgeDataReader.NextResult());
        }
    }
}
