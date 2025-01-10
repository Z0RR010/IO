namespace IO.Modules.Volunteer
{
    public class Organisation
    {
        public List<Volunteer> VolunteerList { get; private set; } = new List<Volunteer>();
        public List<Task> TaskList { get; private set; } = new List<Task>();

        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public char[] PhoneNumber { get; set; }
        public string Address { get; set; }

        public Organisation() { }

        public Organisation(string organisationName, char[] phoneNumber, string address)
        {
            OrganisationName = organisationName;
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
        public void AddTask(Task task) {
            TaskList.Add(task);
        }
    }
}
