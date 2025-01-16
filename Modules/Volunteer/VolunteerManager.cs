using IO.Modules.ResourceManager;

namespace IO.Modules.Volunteer
{
    public class VolunteerManager
    {
        private List<Volunteer> volunteerList;
        private List<Organisation> organisationList;
        private List<Task> taskList;

        public VolunteerManager()
        {
            volunteerList = new List<Volunteer>();
            organisationList = new List<Organisation>();
            taskList = new List<Task>();
        }

        public int VolunteerCount => volunteerList.Count;
        public int OrganisationCount => organisationList.Count;
        public int TaskCount => taskList.Count;
        public List<Organisation> OrganisationList => organisationList;
        public List<Volunteer> VolunteerList => volunteerList;
        public List<Task> TaskList => taskList;

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

        public void AddTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task), "Task cannot be null.");

            if (task.TaskID == 0 && taskList.Count != 0)
            {
                task.TaskID = taskList.Last().TaskID + 1;
            }

            if (taskList.Any(t => t.TaskID == task.TaskID))
            {
                throw new InvalidOperationException($"Task with ID {task.TaskID} already exists.");
            }

            taskList.Add(task);

            var executor = new VolunteerExecuter();

            if (executor.AddTaskToDatabase(task))
            {
                Console.WriteLine("Task added or updated successfully!");
            }
            else
            {
                Console.WriteLine("Failed to add or update task.");
            }
            executor.Dispose();
        }


        public void AddRateToTask(int taskID, Rate rate)
        {
            var task = taskList.FirstOrDefault(t => t.TaskID == taskID);

            if (task == null)
                throw new KeyNotFoundException($"Task with ID {taskID} not found.");

            task.AddRate(rate);
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
            taskList = executor.LoadTaskList();
            executor.Dispose();
        }


    }
}
