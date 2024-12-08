
namespace IO.Modules.Communication;

public class Chat
{
    public Chat(int chatId, List<int> userIds)
    {
        this.ChatId = chatId;
        this.UserIds = userIds;
        Messages = new List<Message>();
    }
    public int ChatId;
    public List<Message> Messages;
    public List<int> UserIds;
}