namespace IO.Modules.Communication;

public interface ICommunication
{

    public bool CreateChat(Permission permission, string[] emails);

    public bool ArchiveChat(Permission permission, int chatId);
    public bool SendMessage(int chatId, string message, string email);

    public bool BringBackChat(Permission permission, int chatId);
    public List<Chat> GetUserChats(string email);

}