using IO.Modules.ResourceManager;

namespace IO.Modules.Volunteer
{
    public class VolunteerManager
    {
        private List<Volunteer> volunteerList;
        private List<Organisation> organisationList;
        private List<VolunteerTask> volunteerTaskList;

        public VolunteerManager()
        {
            volunteerList = new List<Volunteer>();
            organisationList = new List<Organisation>();
            volunteerTaskList = new List<VolunteerTask>();
        }

        public int VolunteerCount => volunteerList.Count;
        public int OrganisationCount => organisationList.Count;
        public int VolunteerTaskCount => volunteerTaskList.Count;
        public List<Organisation> OrganisationList => organisationList;
        public List<Volunteer> VolunteerList => volunteerList;
        public List<VolunteerTask> VolunteerTaskList => volunteerTaskList;

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

        public void AddTask(VolunteerTask volunteerTask, Organisation organisation)
        {
            if (volunteerTask == null)
                throw new ArgumentNullException(nameof(volunteerTask), "VolunteerTask cannot be null.");

            if (volunteerTask.VolunteerTaskID == 0 && volunteerTaskList.Count != 0)
            {
                volunteerTask.VolunteerTaskID = volunteerTaskList.Last().VolunteerTaskID + 1;
            }

            if (volunteerTaskList.Any(t => t.VolunteerTaskID == volunteerTask.VolunteerTaskID))
            {
                throw new InvalidOperationException($"VolunteerTask with ID {volunteerTask.VolunteerTaskID} already exists.");
            }

            volunteerTaskList.Add(volunteerTask);
            FindOrganisationByID(organisation.OrganisationID).AddTask(volunteerTask);
            var executor = new VolunteerExecuter();

            if (executor.AddTaskToDatabase(volunteerTask))
            {
                Console.WriteLine("VolunteerTask added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteerTask.");
            }
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


        public void AddRateToTask(int taskID, Rate rate)
        {
            var volunteerTask = volunteerTaskList.FirstOrDefault(t => t.VolunteerTaskID == taskID);

            if (volunteerTask == null)
                throw new KeyNotFoundException($"VolunteerTask with ID {taskID} not found.");

            volunteerTask.AddRate(rate);
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


        public void Load()
        {
            var executor = new VolunteerExecuter();
            organisationList = executor.LoadOrganisationList();
            volunteerList = executor.LoadVolunteerList(organisationList);
            volunteerTaskList = executor.LoadTaskList();
            executor.Dispose();
        }


    }
}
