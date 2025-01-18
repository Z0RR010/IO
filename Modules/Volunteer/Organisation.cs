namespace IO.Modules.Volunteer
{
    public class Organisation
    {
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string KRS { get; set; }

        public Organisation() { }

        public Organisation(string organisationName, string email, string phoneNumber, string address, string website, string kRS)
        {
            OrganisationName = organisationName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            Website = website;
            KRS = kRS;
        }
    }
}
