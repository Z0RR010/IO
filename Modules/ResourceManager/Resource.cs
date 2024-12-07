using System.Text.Json.Serialization;

namespace ResourceManager;

public class Resource
{
    public String Name { get; set; }
    
    public Category Category { get; set; }
    
    public int Amount { get; set; }
    
    public bool Status { get; set; }

    public Resource(string name, Category category, int amount)
    {
        Name = name;
        Category = category;
        Amount = amount;
        Status = true;
    }

    [JsonConstructor]
    public Resource(string name, Category category, int amount, bool status)
    {
        Name = name;
        Category = category;
        Amount = amount;
        Status = status;
    }

    public void ChangeAmount(int amount)
    {
        Amount = amount;
    }
    
}