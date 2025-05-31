namespace ChefControl.Application.SharedContext.Contracts;

/// <summary>
/// Define um contrato para um serviço de hash que suporta hash e verificação de senhas.
/// </summary>
public interface IHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashPassword);
}