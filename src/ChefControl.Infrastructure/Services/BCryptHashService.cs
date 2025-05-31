using ChefControl.Application.SharedContext.Contracts;

namespace ChefControl.Infrastructure.Services;

public class BCryptHashService : IHashService
{
    public string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hashPassword)
        => BCrypt.Net.BCrypt.Verify(password, hashPassword);
}