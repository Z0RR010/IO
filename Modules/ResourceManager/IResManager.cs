namespace IO.Modules.ResourceManager
{
    /// <summary>
    /// API for working with resource databases
    /// </summary>
    public interface IResManager
    {
        public bool AddItems(string name, string category, int amount, string donorEmail);

        public string GetItem(string name, int amount);

        public int GetItemAmountByCategory(string category);
        
        public List<string> GetItemsByCategory(string category);

        public List<string> GetAllAvailableItems();

        public bool ChangeItemStatus(string name, bool newStatus);

        public bool ChangeItemAmount(string name, int newAmount);
        
        public List<Resource> GetItemsByDonor(string donorEmail);

        public string CustomQuery(string query);

        public Resource GetResourceByName(string name);
    }
}