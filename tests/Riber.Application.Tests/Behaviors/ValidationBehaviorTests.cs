using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Riber.Application.Behaviors;
using Riber.Application.Tests.Behaviors.TestModels;
using Riber.Domain.Tests;
using ApplicationLayer = Riber.Application.Exceptions;

namespace Riber.Application.Tests.Behaviors;

public sealed class ValidationBehaviorTests : BaseTest
{
    private readonly Mock<IValidator<RequestTest>> _mockValidator;
    private readonly RequestTest _request;
    private readonly ResponseTest _response;

    public ValidationBehaviorTests()
    {
        _mockValidator = new Mock<IValidator<RequestTest>>();

        _request = CreateFaker<RequestTest>().CustomInstantiator(f
            => new RequestTest(f.Person.FullName, f.Random.Int(20, 50)));

        _response = CreateFaker<ResponseTest>().CustomInstantiator(f
            => new ResponseTest(f.Person.FullName));
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should proceed to next handler when no validators are provided")]
    public async Task Handle_WhenNoValidatorsProvided_ShouldProceedToNextHandler()
    {
        // Arrange
        var validators = Enumerable.Empty<IValidator<RequestTest>>();
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validators);

        // Act
        var result = await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        result.Should().Be(_response);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should proceed to next handler when validation passes")]
    public async Task Handle_WhenValidationPasses_ShouldProceedToNextHandler()
    {
        //Arrange
        _mockValidator.Setup(x => x.Validate(
                It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult(new List<ValidationFailure>()));

        var validators = new List<IValidator<RequestTest>> { _mockValidator.Object };
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validators);

        // Act
        var result = await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        result.Should().Be(_response);
        _mockValidator.Verify(x => x.Validate(
            It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw ValidationException when single validator fails")]
    public async Task Handle_WhenSingleValidatorFails_ShouldThrowValidationException()
    {
        // Arrange
        List<ValidationFailure> validationFailures =
        [
            new("Name", "Name is required"),
            new("Age", "Age must be between 20 and 50")
        ];

        _mockValidator.Setup(x => x.Validate(
                It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult(validationFailures));

        var validator = new List<IValidator<RequestTest>> { _mockValidator.Object };
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validator);

        // Act
        var exception = async () => await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        var thrownException = await exception.Should().ThrowExactlyAsync<ApplicationLayer.ValidationException>();
        
        thrownException.Which.Details.Should().NotBeNull();
        thrownException.Which.Details.Should().HaveCount(2);
        thrownException.Which.Details.Should().ContainKey("Name");
        thrownException.Which.Details.Should().ContainKey("Age");
        thrownException.Which.Details["Name"].Should().Equal("Name is required");
        thrownException.Which.Details["Age"].Should().Equal("Age must be between 20 and 50");

        _mockValidator.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should throw ValidationException when multiple validators fail")]
    public async Task Handle_WhenMultipleValidatorsFail_ShouldThrowValidationException()
    {
        // Arrange
        var mockValidator1 = new Mock<IValidator<RequestTest>>();
        var mockValidator2 = new Mock<IValidator<RequestTest>>();

        mockValidator1.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Name", "Name is required")
            ]));

        mockValidator2.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Age", "Age must be positive"),
                new ValidationFailure("Name", "Name must be unique")
            ]));

        var validators = new[] { mockValidator1.Object, mockValidator2.Object };
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validators);

        // Act
        var exception = async () => await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        var thrownException = await exception.Should().ThrowExactlyAsync<ApplicationLayer.ValidationException>();
        
        thrownException.Which.Details.Should().NotBeNull();
        thrownException.Which.Details.Should().HaveCount(2);
        thrownException.Which.Details.Should().ContainKey("Name");
        thrownException.Which.Details.Should().ContainKey("Age");
        
        // Name tem 2 erros agrupados
        thrownException.Which.Details["Name"].Should().HaveCount(2);
        thrownException.Which.Details["Name"].Should().Contain("Name is required");
        thrownException.Which.Details["Name"].Should().Contain("Name must be unique");
        
        // Age tem 1 erro
        thrownException.Which.Details["Age"].Should().Equal("Age must be positive");

        mockValidator1.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
        mockValidator2.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should collect errors from all validators and throw single ValidationException")]
    public async Task Handle_WhenCollectingErrorsFromAllValidators_ShouldThrowSingleValidationException()
    {
        // Arrange
        var mockValidator1 = new Mock<IValidator<RequestTest>>();
        var mockValidator2 = new Mock<IValidator<RequestTest>>();
        var mockValidator3 = new Mock<IValidator<RequestTest>>();

        mockValidator1.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Name", "Name is required")
            ]));

        mockValidator2.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult(new List<ValidationFailure>()));

        mockValidator3.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Age", "Age must be positive")
            ]));

        var validators = new[] { mockValidator1.Object, mockValidator2.Object, mockValidator3.Object };
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validators);

        // Act
        var exception = async () => await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        var thrownException = await exception.Should().ThrowExactlyAsync<ApplicationLayer.ValidationException>();
        
        thrownException.Which.Details.Should().NotBeNull();
        thrownException.Which.Details.Should().HaveCount(2);
        thrownException.Which.Details.Should().ContainKey("Name");
        thrownException.Which.Details.Should().ContainKey("Age");
        thrownException.Which.Details["Name"].Should().Equal("Name is required");
        thrownException.Which.Details["Age"].Should().Equal("Age must be positive");

        mockValidator1.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
        mockValidator2.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
        mockValidator3.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Should validate all validators even when some fail")]
    public async Task Handle_WhenSomeValidatorsFail_ShouldValidateAllValidators()
    {
        // Arrange
        var mockValidator1 = new Mock<IValidator<RequestTest>>();
        var mockValidator2 = new Mock<IValidator<RequestTest>>();

        mockValidator1.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Name", "Name is required")
            ]));

        mockValidator2.Setup(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()))
            .Returns(new ValidationResult([
                new ValidationFailure("Age", "Age must be positive")
            ]));

        var validators = new[] { mockValidator1.Object, mockValidator2.Object };
        var behavior = new ValidationBehavior<RequestTest, ResponseTest>(validators);

        // Act
        var exception = async () => await behavior.Handle(
            _request,
            (_, _) => ValueTask.FromResult(_response),
            CancellationToken.None
        );

        // Assert
        var thrownException = await exception.Should().ThrowExactlyAsync<ApplicationLayer.ValidationException>();
        
        // Verifica que TODOS os validators foram chamados
        mockValidator1.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
        mockValidator2.Verify(x => x.Validate(It.IsAny<ValidationContext<RequestTest>>()), Times.Once);
        
        // Verifica que os erros foram agrupados corretamente
        thrownException.Which.Details.Should().HaveCount(2);
        thrownException.Which.Details.Should().ContainKey("Name");
        thrownException.Which.Details.Should().ContainKey("Age");
    }
}