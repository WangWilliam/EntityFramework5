﻿namespace System.Data.Entity.Core.Objects
{
    using System.Data.Entity.Internal;
    using System.Data.Entity.Resources;
    using Moq;
    using Xunit;

    public class ObjectResultTests
    {
        [Fact]
        public void GetEnumerator_calls_Shaper_GetEnumerator()
        {
            var shaperMock = MockHelper.CreateShaperMock<object>();
            var expectedEnumerator = new Mock<IDbEnumerator<object>>().Object;
            shaperMock.Setup(m => m.GetEnumerator()).Returns(() => expectedEnumerator);
            var objectResultMock = new Mock<ObjectResult<object>>(shaperMock.Object, null, null)
            {
                CallBase = true
            };

            var actualEnumerator = objectResultMock.Object.GetEnumerator();

            Assert.Same(expectedEnumerator, actualEnumerator);
            shaperMock.Verify(m => m.GetEnumerator(), Times.Once());
        }

        [Fact]
        public void GetEnumerator_throws_when_called_twice()
        {
            var shaperMock = MockHelper.CreateShaperMock<object>();
            var expectedEnumerator = new Mock<IDbEnumerator<object>>().Object;
            shaperMock.Setup(m => m.GetEnumerator()).Returns(() => expectedEnumerator);
            var objectResultMock = new Mock<ObjectResult<object>>(shaperMock.Object, null, null)
            {
                CallBase = true
            };

            objectResultMock.Object.GetEnumerator();

            Assert.Equal(Strings.Materializer_CannotReEnumerateQueryResults,
                Assert.Throws<InvalidOperationException>(() => objectResultMock.Object.GetEnumerator()).Message);
        }
    }
}