using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace IO.Modules.Volunteer
{
    public class VolunteerManager
    {
        private List<Volunteer> volunteerList;
        private List<Organisation> organisationList;

        public VolunteerManager()
        {
            this.volunteerList = new List<Volunteer>();
            this.organisationList = new List<Organisation>();
            //LoadVolunteersFromFile("volunteers.txt");
        }

        public void addOrganisation(Organisation organisation)
        {
            if (organisation == null)
                throw new ArgumentNullException(nameof(organisation), "Organisation cannot be null.");


            if (organisation.getOrganisationID() == 0 && organisationList.Count != 0)
            {
                organisation.setOrganisationID(organisationList.Last().getOrganisationID() + 1);
            }
            bool flag = false;
            foreach (Organisation org in organisationList)
            {
                if (org.getOrganisationID() == organisation.getOrganisationID())
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                organisationList.Add(organisation);
            }
            else
            {
                throw new InvalidOperationException($"Organisation with ID {organisation.getOrganisationID()} already exists.");
            }
        }


        public void addVolunteer(Volunteer volunteer)
        {

            if (volunteer == null)
            {
                throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");
            }
            Organisation organisation = volunteer.getOrganisation();
            if (organisation == null)
            {
                throw new ArgumentNullException(nameof(organisation), "Organisation cannot be null.");
            }

            if (volunteer.getVolunteerID() == 0 && volunteerList.Count != 0)
            {
                volunteer.setVolunteerID(volunteerList.Last().getVolunteerID() + 1);
            }

            if (volunteerList.Any(v => v.getVolunteerID() == volunteer.getVolunteerID()))
            {
                throw new InvalidOperationException($"Volunteer with ID {volunteer.getVolunteerID()} already exists.");
            }
            else if (!organisationList.Contains(organisation))
            {
                throw new InvalidOperationException("Organisation not found.");
            }
            else
            {
                organisation.addVolunteer(volunteer);
                volunteerList.Add(volunteer);
            }

        }


        public Organisation findOrganisationByID(int organisationID)
        {
            return organisationList.FirstOrDefault(o => o.getOrganisationID() == organisationID);
        }

        public void removeVolunteer(int volunteerID)
        {
            var volunteerToRemove = volunteerList.FirstOrDefault(v => v.getVolunteerID() == volunteerID);

            if (volunteerToRemove == null)
                throw new KeyNotFoundException($"Volunteer with ID {volunteerID} not found.");

            volunteerList.Remove(volunteerToRemove);
        }

        public Volunteer findVolunteerByID(int volunteerID)
        {
            return volunteerList.FirstOrDefault(v => v.getVolunteerID() == volunteerID);
        }

        public int getVolunteerCount()
        {
            return volunteerList.Count;
        }

        public int getOrganisationCount()
        {
            return organisationList.Count;
        }

        public List<Organisation> getOrganisationList()
        {
            return organisationList;
        }
        public string saveManagerToFile()
        {
            string all = string.Empty, volunteers = string.Empty, organisations = string.Empty;

            foreach (Organisation org in organisationList)
            {
                organisations += $"Organisation Details:\n" +
                                 $"ID: {org.getOrganisationID()}\n" +
                                 $"Name: {org.getOrganisationName()}\n" +
                                 $"Phone: {new string(org.getPhoneNumber())}\n" +
                                 $"Address: {org.getAddress()}\n\n";

                foreach (Volunteer vol in org.getVolunteerList())
                {
                    volunteers += $"Volunteer Details:\n" +
                                  $"ID: {vol.getVolunteerID()}\n" +
                                  $"First Name: {vol.getFirstName()}\n" +
                                  $"Last Name: {vol.getLastName()}\n" +
                                  $"Email: {vol.getEmail()}\n" +
                                  $"Gender: {vol.getGender()}\n" +
                                  $"Phone: {new string(vol.getPhoneNumber())}\n" +
                                  $"Address: {vol.getAddress()}\n" +
                                  $"Experience: {vol.getExperience()}\n" +
                                  $"Additional Info: {vol.getAdditionalInfo()}\n" +
                                  $"Skills: {vol.getSkills()}\n" +
                                  $"Availability: {string.Join(", ", vol.getAvailability().Select(a => a.ToString("yyyy-MM-dd")))}\n" +
                                  $"Organisation ID: {org.getOrganisationID()}\n\n";
                }
            }

            all = organisations + volunteers;
            return all;
        }



        public void loadManagerFromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The file at {filePath} does not exist.");

            var lines = File.ReadAllLines(filePath);

            List<Organisation> tempOrganisations = new List<Organisation>();
            List<Volunteer> tempVolunteers = new List<Volunteer>();

            Organisation currentOrganisation = null;
            Volunteer currentVolunteer = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                

                if (trimmedLine.StartsWith("Volunteer Details:"))
                {
                    break;
                }
                if (trimmedLine.StartsWith("Organisation Details:"))
                {
                    // Save the current organisation, if any
                    if (currentOrganisation != null)
                        tempOrganisations.Add(currentOrganisation);

                    currentOrganisation = new Organisation("", new char[] { }, "");
                }
                else if (currentOrganisation != null)
                {
                    if (trimmedLine.StartsWith("ID:"))
                        currentOrganisation.setOrganisationID(int.Parse(trimmedLine.Substring(4)));
                    else if (trimmedLine.StartsWith("Name:"))
                        currentOrganisation.setOrganisationName(trimmedLine.Substring(6));
                    else if (trimmedLine.StartsWith("Phone:"))
                        currentOrganisation.setPhoneNumber(trimmedLine.Substring(7).ToCharArray());
                    else if (trimmedLine.StartsWith("Address:"))
                        currentOrganisation.setAddress(trimmedLine.Substring(9));
                }

            }

            // Add the last organisation if not already added
            if (currentOrganisation != null)
                tempOrganisations.Add(currentOrganisation);
            // Add organisations and volunteers to the manager
            
            currentOrganisation = null;

            foreach (var org in tempOrganisations)
                this.addOrganisation(org);


            bool flag = true;
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("Organisation Details:"))
                {
                    flag = false;
                }

                if (trimmedLine.StartsWith("Volunteer Details:"))
                {
                    // Save the current volunteer, if any
                    if (currentVolunteer != null)
                        tempVolunteers.Add(currentVolunteer);
                    flag = true;
                    currentVolunteer = new Volunteer("", "", "", Gender.Female, new char[] { }, "", "", "", "", new List<DateTime>(), null);
                }

                else if (currentVolunteer != null && flag)
                {
                    if (trimmedLine.StartsWith("ID:"))
                        currentVolunteer.setVolunteerID(int.Parse(trimmedLine.Substring(4)));
                    else if (trimmedLine.StartsWith("First Name:"))
                        currentVolunteer.setFirstName(trimmedLine.Substring(12));
                    else if (trimmedLine.StartsWith("Last Name:"))
                        currentVolunteer.setLastName(trimmedLine.Substring(11));
                    else if (trimmedLine.StartsWith("Email:"))
                        currentVolunteer.setEmail(trimmedLine.Substring(7));
                    else if (trimmedLine.StartsWith("Gender:"))
                    {
                        Enum.TryParse(trimmedLine.Substring(8), out Gender gender);
                        currentVolunteer.setGender(gender);
                    }
                    else if (trimmedLine.StartsWith("Phone:"))
                        currentVolunteer.setPhoneNumber(trimmedLine.Substring(7).ToCharArray());
                    else if (trimmedLine.StartsWith("Address:"))
                        currentVolunteer.setAddress(trimmedLine.Substring(9));
                    else if (trimmedLine.StartsWith("Experience:"))
                        currentVolunteer.setExperience(trimmedLine.Substring(12));
                    else if (trimmedLine.StartsWith("Additional Info:"))
                        currentVolunteer.setAdditionalInfo(trimmedLine.Substring(17));
                    else if (trimmedLine.StartsWith("Skills:"))
                        currentVolunteer.setSkills(trimmedLine.Substring(8));
                    else if (trimmedLine.StartsWith("Availability:"))
                    {
                        var dates = trimmedLine.Substring(14).Split(", ").Select(DateTime.Parse).ToList();
                        currentVolunteer.setAvailability(dates);
                    }
                    else if (trimmedLine.StartsWith("Organisation ID:"))
                    {
                        int organisationID = int.Parse(trimmedLine.Substring(16));
                        Console.WriteLine(organisationID);
                        var organisation = organisationList.FirstOrDefault(org => org.getOrganisationID() == organisationID);
                        if (organisation != null)
                            currentVolunteer.setOrganisation(organisation);
                    }
                }
            }

            // Add the last volunteer if not already added
            if (currentVolunteer != null)
                tempVolunteers.Add(currentVolunteer);

            foreach (var vol in tempVolunteers)
                this.addVolunteer(vol);
        }

        





    }
}
    
