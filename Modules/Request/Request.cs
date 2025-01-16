using IO.Modules.Security;
using IO.Modules.ResourceManager;

namespace RequestModule
{
    public class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DateUpdated { get; set; }
        public RequestStatus Status { get; set; }
        public User User { get; set; }
        public Address Address { get; set; }
        public ICollection<Resource> ResourcesRequired { get; set; } = new List<Resource>();
        public bool IsVerified { get; set; }
        public string HandlingOrganization { get; set; }
    }
}