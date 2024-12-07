using Microsoft.AspNetCore.Identity;

namespace IO.Modules.Volunteer
{
    public class Volunteer 
    {
        private int volunteerID;
        private string firstName;
        private string lastName;
        private string email;
        private Gender gender;
        private char[] phoneNumber;
        private string address;
        private string experience;
        private string additionalInfo;
        private string skills;
        private List<DateTime> availability;

        private Organisation organisation;

        public Volunteer(string firstName, string lastName, string email, Gender gender, char[] phoneNumber,
            string address, string experience, string additionalInfo, string skills, List<DateTime> availability, Organisation organisation)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            this.email = email;
            this.gender = gender;
            this.phoneNumber = phoneNumber;
            this.address = address;
            this.experience = experience;
            this.additionalInfo = additionalInfo;
            this.skills = skills;
            this.availability = availability;
            this.organisation = organisation;
        }





        ///////////////////////////////////ZW///////////////////ZW////////////////ZW////////////////


        public void setVolunteerID(int volunteerID)
        {
            this.volunteerID = volunteerID;
        }

        public int getVolunteerID()
        {
            return volunteerID;
        }

        public string getFirstName()
        {
            return firstName;
        }
        public void setFirstName(string firstName)
        {
            this.firstName = firstName;
        }

        public string getLastName()
        {
            return lastName;
        }
        public void setLastName(string lastName)
        {
            this.lastName = lastName;
        }

        public string getEmail()
        {
            return email;
        }
        public void setEmail(string email)
        {
            this.email = email;
        }

        public Gender getGender()
        {
            return gender;
        }
        public void setGender(Gender gender)
        {
            this.gender = gender;
        }

        public char[] getPhoneNumber()
        {
            return phoneNumber;
        }
        public void setPhoneNumber(char[] phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public string getAddress()
        {
            return address;
        }
        public void setAddress(string address)
        {
            this.address = address;
        }

        public string getExperience()
        {
            return experience;
        }
        public void setExperience(string experience)
        {
            this.experience = experience;
        }

        public string getAdditionalInfo()
        {
            return additionalInfo;
        }
        public void setAdditionalInfo(string additionalInfo)
        {
            this.additionalInfo = additionalInfo;
        }

        public string getSkills()
        {
            return skills;
        }
        public void setSkills(string skills)
        {
            this.skills = skills;
        }

        public List<DateTime> getAvailability()
        {
            return availability;
        }
        public void setAvailability(List<DateTime> availability)
        {
            this.availability = availability;
        }

        public Organisation getOrganisation()
        {
            return organisation;
        }
        public void setOrganisation(Organisation organisation)
        {
            this.organisation = organisation;
        }

    }
}
