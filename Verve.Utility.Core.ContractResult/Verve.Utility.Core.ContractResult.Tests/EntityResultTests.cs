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

            Assert.Equal(string.Empty, result.Entity.FirstName);
            Assert.Null(result.Entity.LastName);
            Assert.Equal(Guid.Empty, result.Entity.Id);
            Assert.Null(result.Entity.DateOfBirth);
        }

        [Fact]
        public void ShouldReturnNotNullableEntityWhenFail()
        {
            var result = Result<TestPerson>.Failure("Error");

            Assert.NotNull(result);

            Assert.Equal(string.Empty, result.Entity.FirstName);
            Assert.Null(result.Entity.LastName);
            Assert.Equal(Guid.Empty, result.Entity.Id);
            Assert.Null(result.Entity.DateOfBirth);
        }

        [Fact]
        public void ReturnValueTypeResultWithDefaultContentFromOtherFailure()
        {

            var result = new Result<TestPerson>(false, ReasonCode.BadRequest, "Bad Request", "Test Failure", default!);

            var failedResult = Result<int>.FailedFromOtherFailed(result);

            Assert.NotNull(failedResult);

            Assert.Equal(default, failedResult.Entity);
        }

        [Fact]
        public void ReturnNotNullContentForRefType()
        {
            var result = Result<string>.Failure("Test error", "test detail error", ReasonCode.BadRequest);

            var objectResult = Result<TestPerson>.FailedFromOtherFailed(result);

            Assert.NotNull(objectResult);
        }

        [Fact]
        public void ShouldReturnFailedErrorWhenOtherResultIsNull()
        {
            var result = Result<TestPerson>.From(null);

            Assert.NotNull(result);
            Assert.True(result.Failed);
        }

        [Fact]
        public void ShouldReturnFailedErrorWhenOtherResultIsFailed()
        {
            var result = Result<TestPerson>.From(Result.Failure("Some Error"));

            Assert.NotNull(result);
            Assert.True(result.Failed);
        }

        [Fact]
        public void ShouldReturnSuccessfulWhenOtherResultIsSuccessful()
        {
            var result = Result<TestPerson>.From(Result<TestPerson>.Success(CreatePerson()));

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Entity);
            Assert.Equal("Test", result.Entity.FirstName);
        }

        [Fact]
        public void ShouldReturnSuccessfulForSubClassWhenOtherResultIsSuccessful()
        {
            var result = Result<TestPerson>.From(Result<Manager>.Success(new Manager
            {
                FirstName = "Manager",
                LastName = "Test"
            }));

            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Entity);
            Assert.Equal("Manager", result.Entity.FirstName);
        }

        [Fact]
        public async Task ShouldReturnFailedIfFirstResultIsFailure()
        {
            var result = Result<string>.Failure("Test error", "test detail error", ReasonCode.BadRequest);

            var newResult = await Result<TestPerson>.CheckResultAndExecuteNextAsync(result, async () => await GetResult());

            Assert.True(newResult.Failed);
            Assert.Equal("Test error", newResult.ErrorMessage);
        }

        [Fact]
        public async Task ShouldReturnSuccessfulIfResultAndAllNextSuccessful()
        {
            var result = Result<string>.Success("Success");

            var newResult = await Result<TestPerson>.CheckResultAndExecuteNextAsync(result, async () => await GetResult());

            Assert.True(newResult.Succeeded);
            Assert.Equal(ReasonCode.Success, newResult.ReasonCode);
        }

        [Fact]
        public void ShouldReturnFailedResultIfOtherEntityResultFailed()
        {
            var result = Result<TestPerson>.Failure("Error Occurred");

            var newResult = Result<TestPersonResponse>.CheckOtherEntityResultAndConvertToResult<TestPerson>(result, p => ConvertPersonToResponse(p));

            Assert.True(newResult.Failed);
        }

        [Fact]
        public void ShouldReturnSuccessResultAndConvertedResponseIfOtherEntityResultSuccess()
        {
            var id = Guid.NewGuid();
            var dob = DateTime.Parse("21/07/1984");

            var result = Result<TestPerson>.Success(new TestPerson
                    {
                        FirstName = "Test",
                        LastName = "Person",
                        Id = id,
                        DateOfBirth=dob,
                    }
            );

            var newResult = Result<TestPersonResponse>.CheckOtherEntityResultAndConvertToResult<TestPerson>(result, p => ConvertPersonToResponse(p));

            Assert.True(newResult.Succeeded);

            var testPerson = newResult.Entity;

            Assert.Equal("Test Person", testPerson.FullName);
            Assert.Equal(id, testPerson.Id);
            Assert.Equal(dob, testPerson.DateOfBirth);
        }


        private static TestPersonResponse ConvertPersonToResponse(TestPerson person)
        {
            return new TestPersonResponse
            {
                Id = person.Id,
                DateOfBirth = person.DateOfBirth,
                FullName = ($"{person.FirstName.Trim()} {person.LastName}").Trim()
            };
        }

        private async Task<Result<TestPerson>> GetResult()
        {
            return await Task.FromResult(Result<TestPerson>
                .Success(new TestPerson
                {
                    FirstName = "Test",
                    LastName = "Person",
                }));
        }

        private TestPerson CreatePerson()
        {
            return new TestPerson
            {
                FirstName = "Test",
                LastName = "Person"
            };
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

    public class Manager : TestPerson
    {
        public Guid ParentId { get; set; }
    }

    public class TestPersonResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        [MaybeNull]
        public DateTime? DateOfBirth { get; set; }
    }
}
