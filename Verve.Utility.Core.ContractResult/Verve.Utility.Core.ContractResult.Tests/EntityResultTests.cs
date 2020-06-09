using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace Verve.Utility.Core.ContractResult.Tests
{
    public class EntityResultTests
    {
        [Fact]
        public void ShouldReturnNotNullableEntity()
        {
            var result = new Result<TestPerson>(true, null, ReasonCode.Success);

            Assert.Equal( string.Empty, result.Entity.FirstName );
            Assert.Null( result.Entity.LastName );
            Assert.Equal( Guid.Empty, result.Entity.Id );
            Assert.Null( result.Entity.DateOfBirth );
        }

        [Fact]
        public void ShouldReturnNotNullableEntityWhenFail()
        {
            var result = Result<TestPerson>.Failure("Error");

            Assert.NotNull( result );

            Assert.Equal( string.Empty, result.Entity.FirstName );
            Assert.Null( result.Entity.LastName );
            Assert.Equal( Guid.Empty, result.Entity.Id );
            Assert.Null( result.Entity.DateOfBirth );
        }

        [Fact]
        public void ReturnValueTypeResultWithDefaultContentFromOtherFailure()
        {

            var result = new Result<TestPerson>(false, ReasonCode.BadRequest, "Bad Request", "Test Failure", default!);

            var failedResult = Result<int>.FailedFromOtherFailed(result);

            Assert.NotNull( failedResult );

            Assert.Equal( default, failedResult.Entity );
        }

        [Fact]
        public void ReturnNotNullContentForRefType()
        {
            var result = Result<string>.Failure("Test error", "test detail error", ReasonCode.BadRequest);

            var objectResult = Result<TestPerson>.FailedFromOtherFailed(result);

            Assert.NotNull( objectResult );
        }

        [Fact]
        public async Task ShouldReturnFailedIfFirstResultIsFailure()
        {
            var result = Result<string>.Failure("Test error", "test detail error", ReasonCode.BadRequest);

           var newResult = await  Result<TestPerson>.CheckResultAndExecuteNextAsync(result, async () => await GetResult());

           Assert.True(newResult.Failed);
           Assert.Equal( "Test error" , newResult.ErrorMessage);
        }

        [Fact]
        public async Task ShouldReturnSuccessfulIfResultandAllNextSuccessful()
        {
            var result = Result<string>.Success("Success");

            var newResult = await  Result<TestPerson>.CheckResultAndExecuteNextAsync(result, async () => await GetResult());

            Assert.True( newResult.Succeeded );
            Assert.Equal( ReasonCode.Success, newResult.ReasonCode );
        }

        private async Task<Result<TestPerson>> GetResult()
        {
            return await Task.FromResult( Result<TestPerson>
                .Success(new TestPerson
                {
                    FirstName = "Test",
                    LastName = "Person",
                }));
        }
    }

    public class TestPerson
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        [MaybeNull]
        public string? LastName { get; set; }

        [MaybeNull]
        public DateTime? DateOfBirth { get; set; }
    }
}
