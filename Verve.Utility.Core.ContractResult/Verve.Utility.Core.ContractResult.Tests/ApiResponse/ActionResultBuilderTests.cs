using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Verve.Utility.Core.ContractResult.ApiResponse;
using Xunit;

namespace Verve.Utility.Core.ContractResult.Tests.ApiResponse
{
    public class ActionResultBuilderTests
    {
        [Fact]
        public async Task ShouldReturnSuccessIfFunctionReturnsSuccessfulEntityResult()
        {
            // Arrange

            Func<Task<Result<TestPerson>>> func = GetSuccessfulEntityResult;

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
        func)! as OkObjectResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 200, result!.StatusCode! );
        }

        private Task<Result<TestPerson>> GetSuccessfulEntityResult()
            => Task.FromResult( Result<TestPerson>.Success( new TestPerson
            {
                FirstName = "Test",
                LastName = "Person"
            } ) );


        [Fact]
        public async Task ShouldReturnBadRequestIfFunctionReturnsBadRequestEntityResult()
        {
            // Arrange
            Func<Task<Result<TestPerson>>> func = GetBadRequestEntityResult;

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
        func) as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( HttpStatusCode.BadRequest, ( HttpStatusCode )( result.StatusCode! ) );
        }

        private Task<Result<TestPerson>> GetBadRequestEntityResult()
            => Task.FromResult( Result<TestPerson>.Failure( "Failed", "Bad Request", ReasonCode.BadRequest ) );


        [Fact]
        public async Task ShouldReturnInternalServerIfFunctionReturnsInternalServerEntityResult()
        {
            // Arrange
            Func<Task<Result<TestPerson>>> func = GetInternalServerErrorEntityResult;

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( HttpStatusCode.InternalServerError, ( HttpStatusCode )( result.StatusCode! ) );
        }

        private Task<Result<TestPerson>> GetInternalServerErrorEntityResult()
            => Task.FromResult( Result<TestPerson>.Failure( "Failed", "Internal Server Error", ReasonCode.InternalServerError ) );

        [Fact]
        public async Task ShouldReturnContentResultWithInternalServerErrorCode_WhenFunctionReturnsDbErrorEntityResult()
        {
            // Arrange

            Task<Result<TestPerson>> Func() => Task.FromResult( Result<TestPerson>.Failure( "Error", ReasonCode.UniqueConstrainError ) );

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 500, result!.StatusCode! );
        }

        [Fact]
        public async Task ShouldReturnContentResultWithInternalServerErrorCode_WhenFunctionReturnsUnknownErrorEntityResult()
        {
            // Arrange

            Task<Result<TestPerson>> Func() => Task.FromResult( Result<TestPerson>.Failure( "Error", ReasonCode.UnknownError ) );

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 500, result!.StatusCode! );
        }

        [Fact]
        public async Task ShouldReturnOkResult_WhenFunctionReturnsSuccessfulResult()
        {
            // Arrange

            Task<Result> Func() => Task.FromResult(Result.Success());

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as OkObjectResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 200, result!.StatusCode! );
        }


        [Fact]
        public async Task ShouldReturnContentResultWithBadRequestStatusCode_WhenFunctionReturnsBadRequestResult()
        {
            // Arrange

            Task<Result> Func() => Task.FromResult( Result.Failure("Error", ReasonCode.BadRequest) );

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 400, result!.StatusCode! );
        }

        [Fact]
        public async Task ShouldReturnContentResultWithInternalServerErrorCode_WhenFunctionReturnsInternalServerErrorResult()
        {
            // Arrange

            Task<Result> Func() => Task.FromResult( Result.Failure( "Error", ReasonCode.InternalServerError ) );

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 500, result!.StatusCode! );
        }

        [Fact]
        public async Task ShouldReturnContentResultWithInternalServerErrorCode_WhenFunctionReturnsDbErrorResult()
        {
            // Arrange

            Task<Result> Func() => Task.FromResult( Result.Failure( "Error", ReasonCode.UniqueConstrainError ) );

            // Act
            var result = await ActionResultBuilder.ExecuteAndBuildResult(
                Func)! as ContentResult;

            // Assert
            Assert.NotNull( result );

            Assert.True( result!.StatusCode.HasValue );

            Assert.Equal( 500, result!.StatusCode! );
        }

    }
}
