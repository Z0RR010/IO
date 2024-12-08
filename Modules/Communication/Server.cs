namespace IO.Modules.Communication;
// temporary until using real server
public class Server
{
    public Server()
    {
        this.ActiveChats = new List<Chat>();
        this.ArchivalChats = new List<Chat>();
        this.Reports = new List<string>();
        this.Permissions = new List<Permission>();
        this.Notyfications = new List<Chat>();
    }

    public List<Chat> ActiveChats;
    public List<Chat> ArchivalChats;
    public List<Permission> Permissions;
    public List<Chat> Notyfications;
    public List<string> Reports;

    //public bool CheckPermission(int userId,Permission permission)
    //{
    //    if (userId > Permissions.Count)
    //    {
    //        return false;
    //    }
    //    return permission == Permissions[userId];
    //}
    
}