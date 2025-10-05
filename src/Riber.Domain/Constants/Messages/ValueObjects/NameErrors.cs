namespace Riber.Domain.Constants.Messages.ValueObjects;

public static class NameErrors
{
    #region FullName

    public const string FullNameFormat = "O nome completo deve conter apenas letras.";
    public const string FullNameEmpty = "O nome completo não pode estar vazio.";

    public static string FullNameLength(byte min, byte max)
        => $"O nome completo deve ter entre {min} e {max} caracteres.";

    #endregion

    #region CorporateName

    public const string CorporateNameEmpty = "O nome corporativo não pode estar vazio.";
    public static string CorporateNameLength(byte min, byte max)
        => $"O nome corporativo deve ter entre {min} e {max} caracteres.";

    #endregion

    #region FantasyName

    public const string FantasyNameEmpty = "O nome fantasia não pode estar vazio.";
    public static string FantasyNameLength(byte min, byte max)
        => $"O nome fantasia deve ter entre {min} e {max} caracteres.";

    #endregion

    #region Username

    public const string UserNameEmpty = "O nome de usuário não pode estar vazio.";

    #endregion
}