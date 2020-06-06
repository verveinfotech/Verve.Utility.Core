using System;
using System.Diagnostics.CodeAnalysis;
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
}
