namespace ResourceManager;

/// <summary>
/// API for working with user databases
/// </summary>
public interface IUserManager
{
    public bool IsUserInDataBase(string email);

    public Individual GetUserFromDataBase(string email);
    
    public bool SendToDataBase(Individual user, string encryptionKey, string password);
    
    public bool IsPasswordCorrect(string email, string password);
    
    public string GetUserPESEL(string email);
}