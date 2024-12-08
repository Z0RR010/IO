namespace IO.Modules.Communication;

public interface ICommunication
{
    
    public bool CreateChat(Permission permission, int[] userIds);

    public bool ArchiveChat(Permission permission, int chatId);
    public bool SendMessage(int chatId, string message, int userId);
    
    public bool BringBackChat(Permission permission, int chatId);
    public List<Chat> GetUserChats(int userId);

}