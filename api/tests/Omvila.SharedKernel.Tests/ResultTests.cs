using Omvila.SharedKernel;

namespace Omvila.SharedKernel.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Success_HasValue_AndNoError()
    {
        Result<int> result = 42;

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Failure_ExposesError_AndReadingValueThrows()
    {
        Result<int> result = Error.Conflict(ErrorCodes.SlotTaken);

        result.IsSuccess.ShouldBeFalse();
        result.Error.Code.ShouldBe("slot_taken");
        result.Error.Kind.ShouldBe(ErrorKind.Conflict);
        Should.Throw<DomainException>(() => result.Value);
    }

    [Fact]
    public void Validation_CarriesFieldErrors()
    {
        var error = Error.Validation("businessName", "required");

        error.Code.ShouldBe(ErrorCodes.ValidationFailed);
        error.FieldErrors["businessName"].ShouldBe(["required"]);
    }

    [Fact]
    public void Error_WithCodeOutsideContract_ThrowsDomainException() =>
        Should.Throw<DomainException>(() => Error.Conflict("made_up_code"));
}
