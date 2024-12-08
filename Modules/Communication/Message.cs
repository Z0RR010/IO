namespace IO.Modules.Communication;

public class Message
{
    public Message(string content, int userId, int chatId)
    {
        this.content = content;
        this.UserId = userId;
        this.ChatId = chatId;

    }
    public string content;
    public int UserId;
    public int ChatId;
}