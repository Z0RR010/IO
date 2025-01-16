namespace IO.Modules.Volunteer
{
    public class Task {
        public int TaskID { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public List<DateTime> Availability { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public Rate Rate { get; set; }

        public Task() { }

        public Task(
            string description,
            string address,
            List<DateTime> availability,
            TaskStatus taskStatus = TaskStatus.New)
        {
            Description = description;
            Address = address;
            Availability = availability;
            TaskStatus = taskStatus;
        }


        public void AddRate(Rate rate)
        {
            Rate = rate;
        }



    }
}
