using System;
using Xunit;

namespace Verve.Utility.Core.ContractResult.Tests
{
    public class ContentResultTest
    {
        [Fact]
        public void ReturnResultWithDefaultContent()
        {
            var result = new ContentResult<string>(true, ReasonCode.Success, null, null, string.Empty);

            Assert.NotNull(result.Content);
            Assert.Equal(string.Empty, result.Content);

        }

        [Fact]
        public void ReturnResultWithDefaultContentFromOtherFailure()
        {
            var result = new ContentResult<TestPerson>(false, ReasonCode.BadRequest, "Bad Request", "Test Failure", default!);

            var stringResult = ContentResult<string>.FailedFromOtherFailed(result);

            Assert.NotNull(stringResult);
            
            Assert.Equal(default, stringResult.Content);
        }

        [Fact]
        public void ReturnValueTypeResultWithDefaultContentFromOtherFailure()
        {
            var result = new ContentResult<TestPerson>(false, ReasonCode.BadRequest, "Bad Request", "Test Failure", default!);

            var stringResult = ContentResult<int>.FailedFromOtherFailed(result);

            Assert.NotNull( stringResult );

            Assert.Equal( default, stringResult.Content );
        }

        [Fact]
        public void ReturnNotNullContentForRefType()
        {
            var result = ContentResult<string>.Failure("Test error", "test detail error", ReasonCode.BadRequest);

            var objectResult = ContentResult<TestPerson>.FailedFromOtherFailed(result);
            
            Assert.NotNull(objectResult);
            Assert.NotNull(objectResult.Content);
            Assert.Equal(Guid.Empty, objectResult.Content.Id);
        }

    }
}