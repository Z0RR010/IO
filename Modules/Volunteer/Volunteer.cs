using Microsoft.AspNetCore.Identity;

namespace IO.Modules.Volunteer
{
    public class Volunteer
    {
        public int VolunteerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public char[] PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Experience { get; set; }
        public string AdditionalInfo { get; set; }
        public string Skills { get; set; }
        public List<DateTime> Availability { get; set; }
        public Organisation Organisation { get; set; }

        public Volunteer(
            string firstName,
            string lastName,
            string email,
            Gender gender,
            char[] phoneNumber,
            string address,
            string experience,
            string additionalInfo,
            string skills,
            List<DateTime> availability,
            Organisation organisation)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Gender = gender;
            PhoneNumber = phoneNumber;
            Address = address;
            Experience = experience;
            AdditionalInfo = additionalInfo;
            Skills = skills;
            Availability = availability;
            Organisation = organisation;
        }
    }
}
