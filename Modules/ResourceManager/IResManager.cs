namespace ResourceManager;

/// <summary>
/// API for working with resource databases
/// </summary>
public interface IResManager
{
    public bool AddItems(string name, string category, int amount);
    
    public string GetItem(string name, int amount);
    
    public int GetItemAmountByCategory(string category);
    
    public List<string> GetAllAvailableItems();

    public bool ChangeItemStatus(string name, bool newStatus);

    public bool ChangeItemAmount(string name, int newAmount);
    
    
}