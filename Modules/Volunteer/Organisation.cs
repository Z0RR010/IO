using IO.Components.Pages;

namespace IO.Modules.Volunteer
{
    public class Organisation
    {
        private List<Volunteer> volunteerList = new List<Volunteer>();

        private int organisationID;
        private string organisationName;
        private char[] phoneNumber;
        private string address;



        public Organisation(string organisationName, char[] phoneNumber, string address)
        {
            this.organisationName = organisationName;
            this.phoneNumber = phoneNumber;
            this.address = address;
        }

        public void addVolunteer(Volunteer volunteer)
        {
            volunteerList.Add(volunteer);
        }

        public void removeVolunteer(Volunteer volunteer)
        {
            volunteerList.Remove(volunteer);
        }
        public void setOrganisationID(int organisationID)
        {
            this.organisationID = organisationID;
        }
        public int getOrganisationID()
        {
            return organisationID;
        }

        public string getOrganisationName()
        {
            return organisationName;
        }
        public void setOrganisationName(string organisationName)
        {
            this.organisationName = organisationName;
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

        public List<Volunteer> getVolunteerList()
        {
            return volunteerList;
        }
    }

}
