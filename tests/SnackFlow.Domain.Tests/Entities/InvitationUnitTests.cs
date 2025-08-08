using FluentAssertions;
using SnackFlow.Domain.Entities;
using SnackFlow.Domain.Enums;

namespace SnackFlow.Domain.Tests.Entities;

public class InvitationUnitTests : BaseTest
{
    [Fact(DisplayName = "Should create invitation successfully with valid data")]
    public void Create_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var email = _faker.Person.Email;
        var companyId = Guid.NewGuid();
        var role = "Manager";
        var permissions = new List<string> { "Read", "Write" };
        var createdByUserId = Guid.NewGuid();
        var position = BusinessPosition.Owner;

        // Act
        var invitation = Invitation.Create(email, companyId, role, permissions, position, createdByUserId);

        // Assert
        invitation.Should().NotBeNull();
        invitation.Id.Should().NotBeEmpty();
        invitation.Email.Value.Should().Be(email.ToLower());
        invitation.CompanyId.Should().Be(companyId);
        invitation.Role.Should().Be(role);
        invitation.Permissions.Should().Be(string.Join(",", permissions));
        invitation.CreatedByUserId.Should().Be(createdByUserId);
        invitation.Position.Should().Be(position);
        invitation.IsUsed.Should().BeFalse();
        invitation.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        invitation.InviteToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = "Should mark invitation as used")]
    public void MarkAsUsed_ShouldSetIsUsedToTrue()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            ["Read"],
            BusinessPosition.Owner,
            Guid.NewGuid()
        );

        // Act
        invitation.MarkAsUsed();

        // Assert
        invitation.IsUsed.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return true if invitation is expired")]
    public void IsExpired_WhenExpired_ShouldReturnTrue()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            ["Read"],
            BusinessPosition.Owner,
            Guid.NewGuid()
        );
        typeof(Invitation).GetProperty("ExpiresAt")!.SetValue(invitation, DateTime.UtcNow.AddSeconds(-1));

        // Act
        var expired = invitation.IsExpired();

        // Assert
        expired.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return false if invitation is not expired")]
    public void IsExpired_WhenNotExpired_ShouldReturnFalse()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            ["Read"],
            BusinessPosition.Owner,
            Guid.NewGuid()
        );

        // Act
        var expired = invitation.IsExpired();

        // Assert
        expired.Should().BeFalse();
    }

    [Fact(DisplayName = "Should return true if invitation is valid (not used and not expired)")]
    public void IsValid_WhenNotUsedAndNotExpired_ShouldReturnTrue()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            ["Read"],
            BusinessPosition.Owner,
            Guid.NewGuid()
        );

        // Act
        var valid = invitation.IsValid();

        // Assert
        valid.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return false if invitation is used")]
    public void IsValid_WhenUsed_ShouldReturnFalse()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            ["Read"],
            BusinessPosition.Owner,
            Guid.NewGuid()
        );
        invitation.MarkAsUsed();

        // Act
        var valid = invitation.IsValid();

        // Assert
        valid.Should().BeFalse();
    }

    [Fact(DisplayName = "Should convert invitation to string using operator")]
    public void Operator_String_ShouldReturnInviteToken()
    {
        // Arrange
        var invitation = Invitation.Create(
            _faker.Person.Email,
            Guid.NewGuid(),
            "Manager",
            new List<string> { "Read" },
            BusinessPosition.Owner,
            Guid.NewGuid()
        );

        // Act
        string invitationString = invitation;

        // Assert
        invitationString.Should().Be(invitation.InviteToken);
    }
}