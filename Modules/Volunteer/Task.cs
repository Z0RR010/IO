namespace IO.Modules.Volunteer
{
    public class VolunteerTask {
        public int VolunteerTaskID { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public List<DateTime> EndDate { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public Rate Rate { get; set; }

        public VolunteerTask() { }

        public VolunteerTask(
            string description,
            string address,
            List<DateTime> endDate,
            TaskStatus taskStatus = TaskStatus.New)
        {
            Description = description;
            Address = address;
            EndDate = endDate;
            TaskStatus = taskStatus;
        }


        public void AddRate(Rate rate)
        {
            Rate = rate;
        }



    }
}
