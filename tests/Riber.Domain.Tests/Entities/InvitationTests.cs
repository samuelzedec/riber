using FluentAssertions;
using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Entities;
using Riber.Domain.Enums;

namespace Riber.Domain.Tests.Entities;

public sealed class InvitationTests : BaseTest
{
   #region Valid Tests

   [Fact(DisplayName = "Should create invitation with valid parameters")]
   [Trait("Category", "Unit")]
   public void Create_WhenValidParameters_ShouldCreateInvitation()
   {
       // Arrange
       var email = _faker.Person.Email;
       var companyId = Guid.NewGuid();
       var role = _faker.Name.JobTitle();
       var permissions = new List<string> { "read", "write", "admin" };
       var position = _faker.Random.Enum<BusinessPosition>();
       var createdByUserId = Guid.NewGuid();

       // Act
       var result = Invitation.Create(email, companyId, role, permissions, position, createdByUserId);

       // Assert
       result.Should().NotBeNull();
       result.Id.Should().NotBe(Guid.Empty);
       result.Email.Value.Should().Be(email.ToLower());
       result.CompanyId.Should().Be(companyId);
       result.Role.Should().Be(role);
       result.Position.Should().Be(position);
       result.CreatedByUserId.Should().Be(createdByUserId);
       result.IsUsed.Should().BeFalse();
       result.Token.Should().NotBeNull();
       result.Token.Value.Should().NotBeNullOrEmpty();
       result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
       result.ExpiresAt.Should().BeOnOrBefore(DateTime.UtcNow.AddDays(2).AddMinutes(1));
       result.Permissions.Should().Be("read,write,admin");
   }

   [Theory(DisplayName = "Should create invitation with different business positions")]
   [Trait("Category", "Unit")]
   [InlineData(BusinessPosition.Manager)]
   [InlineData(BusinessPosition.Employee)]
   [InlineData(BusinessPosition.Director)]
   public void Create_WhenDifferentPositions_ShouldCreateInvitationCorrectly(BusinessPosition position)
   {
       // Arrange
       var email = _faker.Person.Email;
       var companyId = Guid.NewGuid();
       var role = _faker.Name.JobTitle();
       var permissions = new List<string> { "read" };
       var createdByUserId = Guid.NewGuid();

       // Act
       var result = Invitation.Create(email, companyId, role, permissions, position, createdByUserId);

       // Assert
       result.Should().NotBeNull();
       result.Position.Should().Be(position);
   }

   [Fact(DisplayName = "Should create invitation with empty permissions list")]
   [Trait("Category", "Unit")]
   public void Create_WhenEmptyPermissions_ShouldCreateInvitationWithEmptyPermissions()
   {
       // Arrange
       var email = _faker.Person.Email;
       var companyId = Guid.NewGuid();
       var role = _faker.Name.JobTitle();
       var permissions = new List<string>();
       var position = _faker.Random.Enum<BusinessPosition>();
       var createdByUserId = Guid.NewGuid();

       // Act
       var result = Invitation.Create(email, companyId, role, permissions, position, createdByUserId);

       // Assert
       result.Should().NotBeNull();
       result.Permissions.Should().BeEmpty();
   }

   [Fact(DisplayName = "Should generate unique invite tokens for different invitations")]
   [Trait("Category", "Unit")]
   public void Create_WhenMultipleInvitations_ShouldGenerateUniqueTokens()
   {
       // Arrange
       var email1 = _faker.Person.Email;
       var email2 = _faker.Person.Email;
       var companyId = Guid.NewGuid();
       var role = _faker.Name.JobTitle();
       var permissions = new List<string> { "read" };
       var position = _faker.Random.Enum<BusinessPosition>();
       var createdByUserId = Guid.NewGuid();

       // Act
       var invitation1 = Invitation.Create(email1, companyId, role, permissions, position, createdByUserId);
       var invitation2 = Invitation.Create(email2, companyId, role, permissions, position, createdByUserId);

       // Assert
       invitation1.Token.Value.Should().NotBe(invitation2.Token.Value);
       invitation1.Token.Value.Should().NotBeNullOrEmpty();
       invitation2.Token.Value.Should().NotBeNullOrEmpty();
   }

   #endregion

   #region Invalid Tests

   [Theory(DisplayName = "Should throw exception when creating with invalid email")]
   [Trait("Category", "Unit")]
   [InlineData("")]
   [InlineData("   ")]
   [InlineData("invalid-email")]
   [InlineData("@domain.com")]
   [InlineData("user@")]
   public void Create_WhenInvalidEmail_ShouldThrowException(string invalidEmail)
   {
       // Arrange
       var companyId = Guid.NewGuid();
       var role = _faker.Name.JobTitle();
       var permissions = new List<string> { "read" };
       var position = _faker.Random.Enum<BusinessPosition>();
       var createdByUserId = Guid.NewGuid();

       // Act
       var act = () => Invitation.Create(invalidEmail, companyId, role, permissions, position, createdByUserId);

       // Assert
       act.Should().Throw<Exception>();
   }

   #endregion

   #region Status Methods Tests

   [Fact(DisplayName = "Should mark invitation as used")]
   [Trait("Category", "Unit")]
   public void MarkAsUsed_WhenCalled_ShouldSetIsUsedToTrue()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act
       invitation.MarkAsUsed();

       // Assert
       invitation.IsUsed.Should().BeTrue();
       invitation.IsValid().Should().BeFalse();
   }

   [Fact(DisplayName = "Should return false when invitation is not expired")]
   [Trait("Category", "Unit")]
   public void IsExpired_WhenNotExpired_ShouldReturnFalse()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act
       var result = invitation.IsExpired();

       // Assert
       result.Should().BeFalse();
   }

   [Fact(DisplayName = "Should return true when invitation is expired")]
   [Trait("Category", "Unit")]
   public void IsExpired_WhenExpired_ShouldReturnTrue()
   {
       // Arrange
       var invitation = CreateValidInvitation();
       
       // Simular expiração usando reflection para alterar ExpiresAt
       var expiresAtField = typeof(Invitation).GetField("<ExpiresAt>k__BackingField", 
           System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
       expiresAtField?.SetValue(invitation, DateTime.UtcNow.AddDays(-1));

       // Act
       var result = invitation.IsExpired();

       // Assert
       result.Should().BeTrue();
   }

   [Fact(DisplayName = "Should return true when invitation is valid")]
   [Trait("Category", "Unit")]
   public void IsValid_WhenNotUsedAndNotExpired_ShouldReturnTrue()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act
       var result = invitation.IsValid();

       // Assert
       result.Should().BeTrue();
       invitation.IsUsed.Should().BeFalse();
       invitation.IsExpired().Should().BeFalse();
   }

   [Fact(DisplayName = "Should return false when invitation is used")]
   [Trait("Category", "Unit")]
   public void IsValid_WhenUsed_ShouldReturnFalse()
   {
       // Arrange
       var invitation = CreateValidInvitation();
       invitation.MarkAsUsed();

       // Act
       var result = invitation.IsValid();

       // Assert
       result.Should().BeFalse();
   }

   [Fact(DisplayName = "Should return false when invitation is expired")]
   [Trait("Category", "Unit")]
   public void IsValid_WhenExpired_ShouldReturnFalse()
   {
       // Arrange
       var invitation = CreateValidInvitation();
       
       // Simular expiração
       var expiresAtField = typeof(Invitation).GetField("<ExpiresAt>k__BackingField", 
           System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
       expiresAtField?.SetValue(invitation, DateTime.UtcNow.AddDays(-1));

       // Act
       var result = invitation.IsValid();

       // Assert
       result.Should().BeFalse();
   }

   #endregion

   #region Permissions Tests

   [Theory(DisplayName = "Should serialize permissions correctly")]
   [Trait("Category", "Unit")]
   [InlineData(new[] { "read" }, "read")]
   [InlineData(new[] { "read", "write" }, "read,write")]
   [InlineData(new[] { "read", "write", "admin" }, "read,write,admin")]
   [InlineData(new string[0], "")]
   public void SerializePermissions_WhenCalled_ShouldReturnCorrectString(string[] permissions, string expected)
   {
       // Act
       var result = Invitation.SerializePermissions([..permissions]);

       // Assert
       result.Should().Be(expected);
   }

   [Theory(DisplayName = "Should deserialize permissions correctly")]
   [Trait("Category", "Unit")]
   [InlineData("read", new[] { "read" })]
   [InlineData("read,write", new[] { "read", "write" })]
   [InlineData("read,write,admin", new[] { "read", "write", "admin" })]
   [InlineData("", new string[0])]
   public void DeserializePermissions_WhenCalled_ShouldReturnCorrectList(string permissions, string[] expected)
   {
       // Act
       var result = Invitation.DeserializePermissions(permissions);

       // Assert
       result.Should().BeEquivalentTo(expected);
   }

   [Fact(DisplayName = "Should handle permissions with extra commas")]
   [Trait("Category", "Unit")]
   public void DeserializePermissions_WhenExtraCommas_ShouldIgnoreEmptyEntries()
   {
       // Arrange
       var permissions = "read,,write,";

       // Act
       var result = Invitation.DeserializePermissions(permissions);

       // Assert
       result.Should().BeEquivalentTo("read", "write");
   }

   #endregion

   #region Token Tests

   [Fact(DisplayName = "Should generate valid RandomToken")]
   [Trait("Category", "Unit")]
   public void InviteToken_WhenGenerated_ShouldBeValidRandomToken()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act & Assert
       invitation.Token.Should().NotBeNull();
       invitation.Token.Value.Should().NotBeNullOrEmpty();
       
       // Verificar se é base64 válido
       var act = () => Convert.FromBase64String(invitation.Token.Value);
       act.Should().NotThrow();
       
       // Verificar se tem o tamanho esperado
       invitation.Token.Value.Length.Should().BeGreaterThan(40);
   }

   #endregion

   #region Implicit Operator Tests

   [Fact(DisplayName = "Should convert invitation to string using implicit operator")]
   [Trait("Category", "Unit")]
   public void ImplicitOperator_WhenConvertingToString_ShouldReturnInviteTokenValue()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act
       string result = invitation;

       // Assert
       result.Should().Be(invitation.Token.Value);
   }

   [Fact(DisplayName = "Should return invite token value when calling ToString")]
   [Trait("Category", "Unit")]
   public void ToString_WhenCalled_ShouldReturnInviteTokenValue()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Act
       var result = invitation.ToString();

       // Assert
       result.Should().Be(invitation.Token.Value);
   }

   #endregion

   #region Interface Implementation Tests

   [Fact(DisplayName = "Should implement IHasEmail correctly")]
   [Trait("Category", "Unit")]
   public void Invitation_ShouldImplementIHasEmailCorrectly()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Assert
       invitation.Should().BeAssignableTo<IHasEmail>();
       invitation.Email.Should().NotBeNull();
   }

   [Fact(DisplayName = "Should implement IAggregateRoot correctly")]
   [Trait("Category", "Unit")]
   public void Invitation_ShouldImplementIAggregateRootCorrectly()
   {
       // Arrange
       var invitation = CreateValidInvitation();

       // Assert
       invitation.Should().BeAssignableTo<IAggregateRoot>();
   }

   #endregion

   #region Edge Cases

   [Fact(DisplayName = "Should handle single permission correctly")]
   [Trait("Category", "Unit")]
   public void Create_WhenSinglePermission_ShouldSerializeCorrectly()
   {
       // Arrange
       var email = _faker.Person.Email;
       var permissions = new List<string> { "admin" };

       // Act
       var invitation = CreateInvitationWithPermissions(email, permissions);

       // Assert
       invitation.Permissions.Should().Be("admin");
       Invitation.DeserializePermissions(invitation.Permissions).Should().ContainSingle().Which.Should().Be("admin");
   }

   [Fact(DisplayName = "Should handle permissions with spaces correctly")]
   [Trait("Category", "Unit")]
   public void Create_WhenPermissionsWithSpaces_ShouldSerializeCorrectly()
   {
       // Arrange
       var email = _faker.Person.Email;
       List<string> permissions = [ "read data", "write data", "admin panel" ];

       // Act
       var invitation = CreateInvitationWithPermissions(email, permissions);

       // Assert
       invitation.Permissions.Should().Be("read data,write data,admin panel");
       Invitation.DeserializePermissions(invitation.Permissions)
           .Should().BeEquivalentTo("read data", "write data", "admin panel");
   }

   #endregion

   #region Helper Methods

   private Invitation CreateValidInvitation()
   {
       return Invitation.Create(
           _faker.Person.Email,
           Guid.NewGuid(),
           _faker.Name.JobTitle(),
           ["read", "write"],
           _faker.Random.Enum<BusinessPosition>(),
           Guid.NewGuid());
   }

   private Invitation CreateInvitationWithPermissions(string email, List<string> permissions)
   {
       return Invitation.Create(
           email,
           Guid.NewGuid(),
           _faker.Name.JobTitle(),
           permissions,
           _faker.Random.Enum<BusinessPosition>(),
           Guid.NewGuid());
   }

   #endregion
}