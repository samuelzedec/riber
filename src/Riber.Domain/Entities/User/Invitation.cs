using Riber.Domain.Abstractions;
using Riber.Domain.Abstractions.ValueObjects;
using Riber.Domain.Entities.Tenants;
using Riber.Domain.Enums;
using Riber.Domain.ValueObjects.Email;
using Riber.Domain.ValueObjects.RandomToken;

namespace Riber.Domain.Entities.User;

public sealed class Invitation 
    : TenantEntity, IAggregateRoot, IHasEmail, IHasRandomToken
{
    #region Properties

    public Email Email { get; private set; }
    public BusinessPosition Position { get; private set; }
    public string Role { get; private set; }
    public string Permissions { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public RandomToken Token { get; private set; }

    #endregion

    #region Constructors

#pragma warning disable CS8618, CA1823
    private Invitation() : base(Guid.CreateVersion7()) { }
#pragma warning restore CS8618, CA1823

    private Invitation(
        Email email,
        Guid companyId,
        string role,
        string permissions,
        BusinessPosition position,
        Guid createdByUserId) : base(Guid.CreateVersion7())
    {
        Email = email;
        CompanyId = companyId;
        Role = role;
        IsUsed = false;
        Position = position;
        Permissions = permissions;
        CreatedByUserId = createdByUserId;
        ExpiresAt = DateTime.UtcNow.AddDays(2);
        Token = RandomToken.Create();
    }

    #endregion

    #region factories

    public static Invitation Create(
        string email,
        Guid companyId,
        string role,
        List<string> permissions,
        BusinessPosition position,
        Guid createdByUserId)
        => new (
            Email.Create(email),
            companyId,
            role,
            SerializePermissions(permissions),
            position,
            createdByUserId
        );

    #endregion

    #region Methods

    public void MarkAsUsed()
        => IsUsed = true;

    public bool IsExpired()
        => DateTime.UtcNow > ExpiresAt;

    public bool IsValid()
        => !IsUsed && !IsExpired();

    #endregion

    #region Static Methods

    public static string SerializePermissions(List<string> permissions)
        => string.Join(",", permissions);

    public static List<string> DeserializePermissions(string permissions)
        => [..permissions.Split(',', StringSplitOptions.RemoveEmptyEntries)];

    #endregion

    #region Operators

    public static implicit operator string(Invitation invitation)
        => invitation.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Token;

    #endregion
}