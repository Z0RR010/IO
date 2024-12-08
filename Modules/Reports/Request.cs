using ConsoleApp1;
using ResourceManager;

public class Request
{
    public int RequestID { get; set; }
    public User user { get; set; }
    public Address Address { get; set; }
    public string Description { get; set; }
    public RequestStatus Status { get; set; }
    public string ResourcesRequired { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime? DateUpdated { get; set; }
    public bool IsVerified { get; set; }
    public string HandlingOrganization { get; set; } // Tu jakaœ organizacja charytatywna

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    // Konstruktor
    public Request(int userId, string description, string resourcesRequired)
    {
        user = userId; // jakiœ AssignByUserID() ?
        Description = description;
        ResourcesRequired = resourcesRequired;
        DateCreated = DateTime.Now;
        Status = RequestStatus.New;
        IsVerified = false;
    }

    // Metody
    public void Update(string description)
    {
        if (Status == RequestStatus.Accepted || Status == RequestStatus.New)
        {
            if (Description == null)
            {
                throw new ArgumentNullException("Describtion cannot be empty.");
            }
            Description = description;
            Status = RequestStatus.Edited;
            DateUpdated = DateTime.Now;
            IsVerified = false;
        }
    }

    public void Verify()
    {
        IsVerified = true;
        Status = RequestStatus.Accepted;
    }

    public void Delete()
    {
        // Logika usuniêcia (np. ustawienie flagi zamiast faktycznego usuwania)? czy archiwizacja w zale¿noœci od tego czy by³ to ¿art czy faktyczne zg³oszenie
    }

    public RequestStatus CheckStatus()
    {
        return Status;
    }
}
