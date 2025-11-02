using FluentAssertions;
using MassTransit;
using Moq;
using Riber.Infrastructure.Messaging;

namespace Riber.Infrastructure.Tests.Messaging;

public sealed class MassTransitPublishContextWrapperTests
{
    private readonly Mock<PublishContext> _mockPublishContext;
    private readonly MassTransitPublishContextWrapper _wrapper;

    public MassTransitPublishContextWrapperTests()
    {
        _mockPublishContext = new Mock<PublishContext>();
        _wrapper = new MassTransitPublishContextWrapper(_mockPublishContext.Object);
    }


    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetDelay should set Delay property on context")]
    public void SetDelay_ShouldSetDelayPropertyOnContext()
    {
        // Arrange
        var delay = TimeSpan.FromMinutes(5);
        _mockPublishContext.SetupProperty(x => x.Delay);

        // Act
        _wrapper.SetDelay(delay);

        // Assert
        _mockPublishContext.Object.Delay.Should().Be(delay);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetDelay with zero delay should work correctly")]
    public void SetDelay_WithZeroDelay_ShouldSetCorrectly()
    {
        // Arrange
        var delay = TimeSpan.Zero;
        _mockPublishContext.SetupProperty(x => x.Delay);

        // Act
        _wrapper.SetDelay(delay);

        // Assert
        _mockPublishContext.Object.Delay.Should().Be(delay);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetDelay with large delay should work correctly")]
    public void SetDelay_WithLargeDelay_ShouldSetCorrectly()
    {
        // Arrange
        var delay = TimeSpan.FromDays(7);
        _mockPublishContext.SetupProperty(x => x.Delay);

        // Act
        _wrapper.SetDelay(delay);

        // Assert
        _mockPublishContext.Object.Delay.Should().Be(delay);
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetHeader should delegate to context Headers.Set")]
    public void SetHeader_ShouldDelegateToContextHeaders()
    {
        // Arrange
        const string key = "TestHeader";
        const string value = "TestValue";
        var setWasCalled = false;
        var mockHeaders = new Mock<SendHeaders>();

        mockHeaders
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
            .Callback<string, object, bool>((k, v, _) =>
            {
                if (k == key && v.Equals(value))
                    setWasCalled = true;
            });

        _mockPublishContext.Setup(x => x.Headers).Returns(mockHeaders.Object);

        // Act
        _wrapper.SetHeader(key, value);

        // Assert
        setWasCalled.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetHeader with different value types should work correctly")]
    public void SetHeader_WithDifferentValueTypes_ShouldSetCorrectly()
    {
        // Arrange
        const string key = "NumberHeader";
        const int value = 42;
        var setWasCalled = false;
        var mockHeaders = new Mock<SendHeaders>();

        mockHeaders
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
            .Callback<string, object, bool>((k, v, _) =>
            {
                if (k == key && v.Equals(value))
                    setWasCalled = true;
            });

        _mockPublishContext.Setup(x => x.Headers).Returns(mockHeaders.Object);

        // Act
        _wrapper.SetHeader(key, value);

        // Assert
        setWasCalled.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "SetHeader with complex object should work correctly")]
    public void SetHeader_WithComplexObject_ShouldSetCorrectly()
    {
        // Arrange
        const string key = "ObjectHeader";
        var value = new { Name = "Test", Value = 123 };
        var setWasCalled = false;
        var mockHeaders = new Mock<SendHeaders>();

        mockHeaders
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
            .Callback<string, object, bool>((k, v, _) =>
            {
                if (k == key && v == value)
                    setWasCalled = true;
            });

        _mockPublishContext.Setup(x => x.Headers).Returns(mockHeaders.Object);

        // Act
        _wrapper.SetHeader(key, value);

        // Assert
        setWasCalled.Should().BeTrue();
    }

    [Trait("Category", "Unit")]
    [Fact(DisplayName = "Multiple SetHeader calls should set multiple headers")]
    public void SetHeader_MultipleCalls_ShouldSetMultipleHeaders()
    {
        // Arrange
        var callCount = 0;
        var mockHeaders = new Mock<SendHeaders>();

        mockHeaders
            .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
            .Callback(() => callCount++);

        _mockPublishContext.Setup(x => x.Headers).Returns(mockHeaders.Object);

        // Act
        _wrapper.SetHeader("Header1", "Value1");
        _wrapper.SetHeader("Header2", "Value2");
        _wrapper.SetHeader("Header3", 123);

        // Assert
        callCount.Should().Be(3);
    }
}

