namespace IO.Modules.Volunteer
{
    public class Organisation
    {
        public List<Volunteer> VolunteerList { get; private set; } = new List<Volunteer>();
        public List<VolunteerTask> VolunteerTaskList { get; private set; } = new List<VolunteerTask>();

        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string Email { get; set; }
        public char[] PhoneNumber { get; set; }
        public string Address { get; set; }

        public Organisation() { }

        public Organisation(string organisationName, string email, char[] phoneNumber, string address)
        {
            OrganisationName = organisationName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public void AddVolunteer(Volunteer volunteer)
        {
            VolunteerList.Add(volunteer);
        }

        public void RemoveVolunteer(Volunteer volunteer)
        {
            VolunteerList.Remove(volunteer);
        }
        public void AddTask(VolunteerTask volunteerTask) {
            VolunteerTaskList.Add(volunteerTask);
        }
    }
}
