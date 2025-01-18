using IO.Modules.ResourceManager;
using IO.Modules.Security;

namespace IO.Modules.Volunteer
{
    public class VolunteerManager
    {
        private List<Volunteer> volunteerList;
        private List<Organisation> organisationList;
        private List<VolunteerTask> volunteerTaskList;
        private List<Rate> rateList;

        public VolunteerManager()
        {
            volunteerList = new List<Volunteer>();
            organisationList = new List<Organisation>();
            volunteerTaskList = new List<VolunteerTask>();
            rateList = new List<Rate>();
        }
        public List<Organisation> OrganisationList => organisationList;
        public List<Volunteer> VolunteerList => volunteerList;
        public List<VolunteerTask> VolunteerTaskList => volunteerTaskList;
        public List<Rate> RateList => rateList;

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

            if (executor.SendOrganisationToDatabase(organisation))
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

            if (volunteer.OrganisationID == null)
                throw new ArgumentNullException(nameof(volunteer.OrganisationID), "Organisation cannot be null.");

            if (volunteer.VolunteerID == 0 && volunteerList.Count != 0)
            {
                volunteer.VolunteerID = volunteerList.Last().VolunteerID + 1;
            }

            if (volunteerList.Any(v => v.VolunteerID == volunteer.VolunteerID))
            {
                throw new InvalidOperationException($"Volunteer with ID {volunteer.VolunteerID} already exists.");
            }

            if (FindOrganisationByID(volunteer.OrganisationID) == null)
            {
                throw new InvalidOperationException("Organisation not found.");
            }

            volunteerList.Add(volunteer);

            var executor = new VolunteerExecuter();

            if (executor.SendVolunteerToDatabase(volunteer))
            {
                Console.WriteLine("Volunteer added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteer.");
            }
            executor.Dispose();
        }

        public void AddTask(VolunteerTask volunteerTask)
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

            var executor = new VolunteerExecuter();

            if (executor.SendTaskToDatabase(volunteerTask))
            {
                Console.WriteLine("VolunteerTask added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteerTask.");
            }
            executor.Dispose();
        }

        public void AddRate(Rate rate)
        {
            if (rate == null)
                throw new ArgumentNullException(nameof(rate), "Rate cannot be null.");

            if (rate.RateID == 0 && rateList.Count != 0)
            {
                rate.RateID = rateList.Last().RateID + 1;
            }

            if (rateList.Any(r => r.RateID == rate.RateID))
            {
                throw new InvalidOperationException($"Rate with ID {rate.RateID} already exists.");
            }

            rateList.Add(rate);

            var executor = new VolunteerExecuter();

            if (executor.SendRateToDatabase(rate))
            {
                Console.WriteLine("Rate added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update rate.");
            }
            executor.Dispose();
        }

        public void AssignTask(VolunteerTask volunteerTask, Volunteer volunteer)
        {
            VolunteerTask vt = FindTaskByID(volunteerTask.VolunteerTaskID);
            vt.VolunteerID = volunteer.VolunteerID;
            UpdateTaskStatus(vt, TaskStatus.Assigned);
        }

        public void UpdateTaskStatus(VolunteerTask volunteerTask, TaskStatus taskStatus)
        {
            volunteerTask.TaskStatus = taskStatus;
            var executor = new VolunteerExecuter();

            if (executor.SendTaskToDatabase(volunteerTask))
            {
                Console.WriteLine("VolunteerTask added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteerTask.");
            }
            executor.Dispose();
        }

        public void EditOrganisation(Organisation organisation)
        {
            Organisation org = FindOrganisationByID(organisation.OrganisationID);
            org = organisation;
            var executor = new VolunteerExecuter();
            if (executor.SendOrganisationToDatabase(org))
            {
                Console.WriteLine("VolunteerTask added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteerTask.");
            }
            executor.Dispose();
        }

        public void EditVolunteer(Volunteer volunteer)
        {
            Volunteer vol = FindVolunteerByID(volunteer.VolunteerID);
            vol = volunteer;

            var executor = new VolunteerExecuter();
            if (executor.SendVolunteerToDatabase(vol))
            {
                Console.WriteLine("VolunteerTask added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update volunteerTask.");
            }
            executor.Dispose();
        }

        public void AddRateToTask(int taskID, Rate rate)
        {
            var volunteerTask = volunteerTaskList.FirstOrDefault(t => t.VolunteerTaskID == taskID);

            if (volunteerTask == null)
                throw new KeyNotFoundException($"VolunteerTask with ID {taskID} not found.");

        }


        public Organisation FindOrganisationByID(int organisationID)
        {
            return organisationList.FirstOrDefault(o => o.OrganisationID == organisationID);
        }
        public VolunteerTask FindTaskByID(int taskID)
        {
            return volunteerTaskList.FirstOrDefault(t => t.VolunteerTaskID == taskID);
        }
        public Volunteer FindVolunteerByID(int volunteerID)
        {
            return volunteerList.FirstOrDefault(v => v.VolunteerID == volunteerID);
        }
        public List<Volunteer> FindVolunteersByOrganisation(int organisationID)
        {
            return volunteerList.Where(v => v.OrganisationID == organisationID).ToList();
        }

        public List<VolunteerTask> FindTasksByStatus(TaskStatus taskStatus)
        {
            return volunteerTaskList.Where(t => t.TaskStatus == taskStatus).ToList();
        }

        public List<VolunteerTask> FindTasksByOrganisation(int organisationID)
        {
            return volunteerTaskList.Where(t => t.OrganisationID == organisationID).ToList();
        }

        public List<VolunteerTask> FindTasksByVolunteer(int volunteerID)
        {
            return volunteerTaskList.Where(t => t.VolunteerID == volunteerID).ToList();
        }





        public void RemoveVolunteer(int volunteerID)
        {
            var volunteerToRemove = volunteerList.FirstOrDefault(v => v.VolunteerID == volunteerID);

            if (volunteerToRemove == null)
                throw new KeyNotFoundException($"Volunteer with ID {volunteerID} not found.");

            volunteerList.Remove(volunteerToRemove);
        }


        public void Load()
        {
            var executor = new VolunteerExecuter();
            organisationList = executor.LoadOrganisationList();
            volunteerList = executor.LoadVolunteerList();
            volunteerTaskList = executor.LoadTaskList();
            executor.Dispose();
        }


    }
}
