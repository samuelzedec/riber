using System.Security.Cryptography;
using SnackFlow.Domain.Abstractions;
using SnackFlow.Domain.Abstractions.ValueObjects;
using SnackFlow.Domain.Enums;
using SnackFlow.Domain.ValueObjects.Email;

namespace SnackFlow.Domain.Entities;

public sealed class Invitation 
    : BaseEntity, IAggregateRoot, IHasEmail
{
    #region Properties

    public Email Email { get; private set; }
    public Guid CompanyId { get; private set; }
    public BusinessPosition Position { get; private set; }
    public string Role { get; private set; }
    public string Permissions { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public string InviteToken { get; private set; }

    #endregion

    #region Constructors

    private Invitation() : base(Guid.CreateVersion7())
    {
        Email = null!;
        CompanyId = Guid.Empty;
        Role = null!;
        IsUsed = false;
        Position = default;
        Permissions = string.Empty;
        CreatedByUserId = Guid.Empty;
        ExpiresAt = default;
        InviteToken = string.Empty;
    }

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
        InviteToken = GeneratePublicCode();
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
    
    private string GeneratePublicCode()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

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
        => InviteToken;

    #endregion
}