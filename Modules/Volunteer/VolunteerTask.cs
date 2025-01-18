namespace IO.Modules.Volunteer
{
    public class VolunteerTask {
        public int VolunteerTaskID { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime EndDate { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public Rate Rate { get; set; }
        public int OrganisationID { get; set; }
        public int VolunteerID { get; set; }
        public int RequestID { get; set; }

        public VolunteerTask() { }

        public VolunteerTask(
            string description,
            string address,
            DateTime endDate,
            int organisationID,
            int requestID,
            int volunteerID = -1,
            TaskStatus taskStatus = TaskStatus.New)
        {
            Description = description;
            Address = address;
            EndDate = endDate;
            OrganisationID = organisationID;
            RequestID = requestID;
            VolunteerID = volunteerID;
            TaskStatus = taskStatus;
        }


        public void AddRate(Rate rate)
        {
            Rate = rate;
        }
    }
}
