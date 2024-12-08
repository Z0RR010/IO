namespace IO.Modules.Communication;
using System.Linq;
public class Communicator : ICommunication
{
    public Server testServer { get; private set;}
    RaportManager manager = new RaportManager();

    public Communicator() {
        testServer = new Server();
    }

    public bool ArchiveChat(Permission permission, int chatId)
    {
        if (permission != Permission.System)
        {
            return false;
        }
        var archivalChat  = testServer.ActiveChats.Find(x => x.ChatId == chatId);
        if(archivalChat == null) return false;
        testServer.ArchivalChats.Add(archivalChat);
        testServer.ActiveChats.Remove(archivalChat);
        Log("Chat" + Convert.ToString(chatId) + " archived ");
        return true;
    }

    public bool CreateChat(Permission permission, int[] userIds)
    {
        if (permission != Permission.System)
        {
            return false;
        }
        int chatId;
        if (testServer.ActiveChats.Count == 0)
        {
            chatId = 1;
        }
        else
        {
            chatId = testServer.ActiveChats.Max(x => x.ChatId) + 1;
        }
        Chat newChat =new Chat(chatId,userIds.OfType<int>().ToList());
        testServer.ActiveChats.Add(newChat);

        Log("Creation of a chat " + Convert.ToString(chatId));
        return true;
    }

    public bool SendMessage(int chatId, string message,int userId)
    {
        var activeChat  = testServer.ActiveChats.Find(x => x.ChatId == chatId);
        if(activeChat == null) return false;
        Message messageToSend = new Message(message,userId,chatId);
        activeChat.Messages.Add(messageToSend);
        return true;
    }

    public bool BringBackChat(Permission permission, int chatId)
    {
        if (permission != Permission.System)
        {
            return false;
        }
        var archivalChat  = testServer.ArchivalChats.Find(x => x.ChatId == chatId);
        if(archivalChat == null) return false;
        testServer.ActiveChats.Add(archivalChat);
        testServer.ArchivalChats.Remove(archivalChat);
        Log("Chat brought back" + Convert.ToString(chatId));
        return true;
    }

    public List<Chat> GetUserChats(int userId)
    {
        List<Chat> userChats = new List<Chat>();
        for (int i = 0; i < testServer.ActiveChats.Count; i++)
        {
            if (testServer.ActiveChats[i].UserIds.Contains(userId))
            userChats.Add(testServer.ActiveChats[i]);
        }
        return userChats;
    }

    void Log(string log)
    {
        manager.activeReport += log + "     " + DateTime.Now.ToShortTimeString() + "\n";
    }

    
    
}