namespace ResourceManager;

/// <summary>
/// API for working with user databases
/// </summary>
public interface IUserManager
{
    public bool IsUserInDataBase(string email);

    public Individual GetUserFromDataBase(string email);
    
    public bool SendToDataBase(Individual user, string encryptionKey, string password, string token);
    
    public bool IsPasswordCorrect(string email, string password);
    
    public string GetUserPESEL(string email);

    public string CustomQuery(string query);
    
    public string GetEncryptionKey(string email);
    
    public string GetToken(string email);
    
    public bool UpdateEmailVerified(string email, bool value);
}