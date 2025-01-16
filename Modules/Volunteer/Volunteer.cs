namespace IO.Modules.Volunteer
{
    public class Volunteer
    {
        public int VolunteerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public char[] PhoneNumber { get; set; }
        public string Address { get; set; }
        public Organisation Organisation { get; set; }
        public List<Task> TaskList { get; private set; } = new List<Task>();

        public Volunteer() { }

        public Volunteer(
            string firstName,
            string lastName,
            string email,
            char[] phoneNumber,
            string address,
            Organisation organisation)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Organisation = organisation;
        }

        public void AddTask(Task task)
        {
            TaskList.Add(task);
        }
    }
}
