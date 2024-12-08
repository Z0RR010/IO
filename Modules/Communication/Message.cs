namespace IO.Modules.Communication;

public class Message
{
    public Message(string content, string email, int chatId)
    {
        this.content = content;
        this.Email = email;
        this.ChatId = chatId;

    }
    public string content;
    public string Email;
    public int ChatId;
}