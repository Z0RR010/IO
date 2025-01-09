using IO.Modules.ResourceManager;

namespace IO.Modules.Volunteer
{
    public class VolunteerManager
    {
        private List<Volunteer> volunteerList;
        private List<Organisation> organisationList;

        public VolunteerManager()
        {
            volunteerList = new List<Volunteer>();
            organisationList = new List<Organisation>();
        }

        public void AddOrganisation(Organisation organisation)
        {
            if (organisation == null)
                throw new ArgumentNullException(nameof(organisation), "Organisation cannot be null.");

            if (organisation.OrganisationID == 0 && organisationList.Count != 0)
            {
                organisation.OrganisationID = organisationList.Last().OrganisationID + 1;
            }

            if (organisationList.Any(o => o.OrganisationID == organisation.OrganisationID))
            {
                throw new InvalidOperationException($"Organisation with ID {organisation.OrganisationID} already exists.");
            }

            organisationList.Add(organisation);

            var executor = new VolunteerExecuter();

            if (executor.AddOrganisationToDatabase(organisation))
            {
                Console.WriteLine("Organisation added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update organisation.");
            }
            executor.Dispose();

        }

        public void AddVolunteer(Volunteer volunteer)
        {
            if (volunteer == null)
                throw new ArgumentNullException(nameof(volunteer), "Volunteer cannot be null.");

            if (volunteer.Organisation == null)
                throw new ArgumentNullException(nameof(volunteer.Organisation), "Organisation cannot be null.");

            if (volunteer.VolunteerID == 0 && volunteerList.Count != 0)
            {
                volunteer.VolunteerID = volunteerList.Last().VolunteerID + 1;
            }

            if (volunteerList.Any(v => v.VolunteerID == volunteer.VolunteerID))
            {
                throw new InvalidOperationException($"Volunteer with ID {volunteer.VolunteerID} already exists.");
            }

            if (!organisationList.Contains(volunteer.Organisation))
            {
                throw new InvalidOperationException("Organisation not found.");
            }

            volunteer.Organisation.AddVolunteer(volunteer);
            volunteerList.Add(volunteer);

            var executor = new VolunteerExecuter();

            if (executor.AddVolunteerToDatabase(volunteer, volunteer.Organisation))
            {
                Console.WriteLine("Organisation added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update organisation.");
            }
            executor.Dispose();
        }

        public Organisation FindOrganisationByID(int organisationID)
        {
            return organisationList.FirstOrDefault(o => o.OrganisationID == organisationID);
        }

        public void RemoveVolunteer(int volunteerID)
        {
            var volunteerToRemove = volunteerList.FirstOrDefault(v => v.VolunteerID == volunteerID);

            if (volunteerToRemove == null)
                throw new KeyNotFoundException($"Volunteer with ID {volunteerID} not found.");

            volunteerList.Remove(volunteerToRemove);
        }

        public Volunteer FindVolunteerByID(int volunteerID)
        {
            return volunteerList.FirstOrDefault(v => v.VolunteerID == volunteerID);
        }

        public int VolunteerCount => volunteerList.Count;
        public int OrganisationCount => organisationList.Count;
        public List<Organisation> OrganisationList => organisationList;
        public List<Volunteer> VolunteerList => volunteerList;






        public void Load()
        {
            var executor = new VolunteerExecuter();
            organisationList = executor.LoadOrganisationList();
            volunteerList = executor.LoadVolunteerList(organisationList);

            executor.Dispose();
        }











        public string SaveManagerToFile()
        {
            string all = string.Empty, volunteers = string.Empty, organisations = string.Empty;

            foreach (var org in organisationList)
            {
                organisations += $"Organisation Details:\n" +
                                 $"ID: {org.OrganisationID}\n" +
                                 $"Name: {org.OrganisationName}\n" +
                                 $"Phone: {new string(org.PhoneNumber)}\n" +
                                 $"Address: {org.Address}\n\n";

                foreach (var vol in org.VolunteerList)
                {
                    volunteers += $"Volunteer Details:\n" +
                                  $"ID: {vol.VolunteerID}\n" +
                                  $"First Name: {vol.FirstName}\n" +
                                  $"Last Name: {vol.LastName}\n" +
                                  $"Email: {vol.Email}\n" +
                                  $"Gender: {vol.Gender}\n" +
                                  $"Phone: {new string(vol.PhoneNumber)}\n" +
                                  $"Address: {vol.Address}\n" +
                                  $"Experience: {vol.Experience}\n" +
                                  $"Additional Info: {vol.AdditionalInfo}\n" +
                                  $"Skills: {vol.Skills}\n" +
                                  $"Availability: {string.Join(", ", vol.Availability.Select(a => a.ToString("yyyy-MM-dd")))}\n" +
                                  $"Organisation ID: {org.OrganisationID}\n\n";
                }
            }

            return organisations + volunteers;
        }

        public void LoadManagerFromFile(string filePath)
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
                        currentOrganisation.OrganisationID = int.Parse(trimmedLine.Substring(4));
                    else if (trimmedLine.StartsWith("Name:"))
                        currentOrganisation.OrganisationName = trimmedLine.Substring(6);
                    else if (trimmedLine.StartsWith("Phone:"))
                        currentOrganisation.PhoneNumber = trimmedLine.Substring(7).ToCharArray();
                    else if (trimmedLine.StartsWith("Address:"))
                        currentOrganisation.Address = trimmedLine.Substring(9);
                }
            }

            // Add the last organisation if not already added
            if (currentOrganisation != null)
                tempOrganisations.Add(currentOrganisation);

            // Add organisations to the manager
            foreach (var org in tempOrganisations)
                AddOrganisation(org);

            currentOrganisation = null;

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
                        currentVolunteer.VolunteerID = int.Parse(trimmedLine.Substring(4));
                    else if (trimmedLine.StartsWith("First Name:"))
                        currentVolunteer.FirstName = trimmedLine.Substring(12);
                    else if (trimmedLine.StartsWith("Last Name:"))
                        currentVolunteer.LastName = trimmedLine.Substring(11);
                    else if (trimmedLine.StartsWith("Email:"))
                        currentVolunteer.Email = trimmedLine.Substring(7);
                    else if (trimmedLine.StartsWith("Gender:"))
                    {
                        Enum.TryParse(trimmedLine.Substring(8), out Gender gender);
                        currentVolunteer.Gender = gender;
                    }
                    else if (trimmedLine.StartsWith("Phone:"))
                        currentVolunteer.PhoneNumber = trimmedLine.Substring(7).ToCharArray();
                    else if (trimmedLine.StartsWith("Address:"))
                        currentVolunteer.Address = trimmedLine.Substring(9);
                    else if (trimmedLine.StartsWith("Experience:"))
                        currentVolunteer.Experience = trimmedLine.Substring(12);
                    else if (trimmedLine.StartsWith("Additional Info:"))
                        currentVolunteer.AdditionalInfo = trimmedLine.Substring(17);
                    else if (trimmedLine.StartsWith("Skills:"))
                        currentVolunteer.Skills = trimmedLine.Substring(8);
                    else if (trimmedLine.StartsWith("Availability:"))
                    {
                        //var times = trimmedLine.Substring(14)
                        //    .Split(", ")
                        //    .Select(time => DateTime.ParseExact(time, @"hh\:mm\:ss", null))
                        //    .ToList();
                        //currentVolunteer.Availability = times;
                        var dates = trimmedLine.Substring(14).Split(", ").Select(DateTime.Parse).ToList();
                        currentVolunteer.Availability = dates;
                    }
                    else if (trimmedLine.StartsWith("Organisation ID:"))
                    {
                        int organisationID = int.Parse(trimmedLine.Substring(16));
                        var organisation = organisationList.FirstOrDefault(org => org.OrganisationID == organisationID);
                        if (organisation != null)
                            currentVolunteer.Organisation = organisation;
                    }
                }
            }

            // Add the last volunteer if not already added
            if (currentVolunteer != null)
                tempVolunteers.Add(currentVolunteer);

            // Add volunteers to the manager
            foreach (var vol in tempVolunteers)
                AddVolunteer(vol);
        }

    }
}
