using IO.Modules.Volunteer;
namespace IO.Modules.ResourceManager
{
    // <summary>
    /// API for working with volunteer databases
    /// </summary>
    public interface IVolunteerManager
    {
        public bool AddOrganisationToDatabase(Organisation organisation);
        
        public bool AddVolunteerToDatabase(IO.Modules.Volunteer.Volunteer volunteer, Organisation organisation);
        
        public List<IO.Modules.Volunteer.Volunteer> LoadVolunteerList(List<Organisation> organisationList);
        
        public List<Organisation> LoadOrganisationList();
    }
}