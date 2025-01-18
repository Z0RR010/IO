namespace IO.Modules.Volunteer
{
    public class Volunteer
    {
        public int VolunteerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int OrganisationID { get; set; }

        public Volunteer() { }

        public Volunteer(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string address,
            int organisationID)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            OrganisationID = organisationID;
        }
    }
}
