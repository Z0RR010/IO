
namespace IO.Modules.Communication;

public class Chat
{
    public Chat(int chatId, List<string> emails)
    {
        this.ChatId = chatId;
        this.Emails = emails;
        Messages = new List<Message>();
    }
    public int ChatId;
    public List<Message> Messages;
    public List<string> Emails;
}